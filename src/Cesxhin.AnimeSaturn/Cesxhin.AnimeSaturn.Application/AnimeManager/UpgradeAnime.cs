using Cesxhin.AnimeSaturn.Application.AnimeManager.Interfaces;
using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.HtmlAgilityPack;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cesxhin.AnimeSaturn.Application.AnimeManager
{
    public class UpgradeAnime : IUpgradeAnime
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //variables
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";

        //rabbit
        private readonly IBus _publishEndpoint;

        public UpgradeAnime(IBus publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public void ExecuteUpgrade()
        {
            //list
            List<GenericAnimeDTO> listGenerics = new();
            List<EpisodeRegisterDTO> listEpisodeRegister;
            List<EpisodeRegisterDTO> blacklist;

            //api
            Api<GenericAnimeDTO> genericApi = new();
            Api<EpisodeDTO> episodeApi = new();
            Api<EpisodeRegisterDTO> episodeRegisterApi = new();

            try
            {
                listGenerics = genericApi.GetMore("/anime/all").GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"not found, details: " + ex.Message);
            }

            //step check on website if the anime is still active
            foreach (var list in listGenerics)
            {
                var anime = list.Anime;

                //get list episodes by name
                List<EpisodeDTO> checkEpisodes = null;
                List<EpisodeDTO> listEpisodesAdd = null;

                _logger.Info("Check new episodes for Anime: " + anime.Name);

                //check new episode
                checkEpisodes = HtmlAnimeSaturn.GetEpisodes(anime.UrlPage, anime.Name);

                //check if null
                if (checkEpisodes == null)
                {
                    _logger.Error($"Can't download with this url, {anime.UrlPage}");
                    continue;
                }

                listEpisodesAdd = new(checkEpisodes);
                blacklist = new();

                foreach (var checkEpisode in checkEpisodes)
                {
                    foreach (var episode in list.Episodes)
                    {
                        if (episode.NumberEpisodeCurrent == checkEpisode.NumberEpisodeCurrent)
                        {
                            blacklist.Add(list.EpisodeRegister.Find(e => e.EpisodeId == episode.ID));
                            listEpisodesAdd.Remove(checkEpisode);
                            break;
                        }
                    }
                }

                if (listEpisodesAdd.Count > 0)
                {
                    _logger.Info($"There are new episodes ({listEpisodesAdd.Count}) of {anime.Name}");

                    //insert to db
                    listEpisodesAdd = episodeApi.PostMore("/episodes", listEpisodesAdd).GetAwaiter().GetResult();

                    //create episodeRegister
                    listEpisodeRegister = new();

                    string pathDefault = null;
                    string path = null;

                    if (blacklist.Count > 0)
                        pathDefault = Path.GetDirectoryName(blacklist.FirstOrDefault().EpisodePath);

                    foreach (var episode in listEpisodesAdd)
                    {
                        path = "";
                        //use path how others episodesRegisters
                        if (pathDefault != null)
                        {
                            path = $"{pathDefault}/{episode.AnimeId} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString("D2")}.mp4";
                        }
                        else //default
                        {
                            path = $"{_folder}/{episode.AnimeId}/Season {episode.NumberSeasonCurrent.ToString("D2")}/{episode.AnimeId} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString("D2")}.mp4";
                        }

                        listEpisodeRegister.Add(new EpisodeRegisterDTO
                        {
                            EpisodeId = episode.ID,
                            EpisodePath = path
                        });
                    }

                    episodeRegisterApi.PostMore("/episodes/registers", listEpisodeRegister).GetAwaiter();

                    //create message for notify
                    string message = $"💽UpgradeService say: \nAdd new episode of {anime.Name}\n";

                    listEpisodesAdd.Sort(delegate (EpisodeDTO p1, EpisodeDTO p2) { return p1.NumberEpisodeCurrent.CompareTo(p2.NumberEpisodeCurrent); });
                    foreach (var episodeNotify in listEpisodesAdd)
                    {
                        message += $"- {episodeNotify.AnimeId} Episode: {episodeNotify.NumberEpisodeCurrent}\n";
                    }

                    try
                    {
                        var messageNotify = new NotifyDTO
                        {
                            Message = message,
                            Image = anime.Image
                        };
                        _publishEndpoint.Publish(messageNotify).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
                    }


                    _logger.Info($"Done upgrade! of {anime.Name}");
                }
                //clear resource
                listEpisodesAdd.Clear();
            }
        }
    }
}
