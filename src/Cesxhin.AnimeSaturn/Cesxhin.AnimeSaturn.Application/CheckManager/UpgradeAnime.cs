﻿using Cesxhin.AnimeSaturn.Application.CheckManager.Interfaces;
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

namespace Cesxhin.AnimeSaturn.Application.CheckManager
{
    public class UpgradeAnime : IUpgrade
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //variables
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";

        //list
        private List<GenericAnimeDTO> listGenerics = new();
        private List<EpisodeRegisterDTO> listEpisodeRegister;
        private List<EpisodeRegisterDTO> blacklist;

        //api
        private readonly Api<GenericAnimeDTO> genericApi = new();
        private readonly Api<EpisodeDTO> episodeApi = new();
        private readonly Api<EpisodeRegisterDTO> episodeRegisterApi = new();

        //rabbit
        private readonly IBus _publishEndpoint;


        public UpgradeAnime(IBus publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public void ExecuteUpgrade()
        {
            _logger.Info($"Start upgrade anime");

            try
            {
                listGenerics = genericApi.GetMore("/anime/all").GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"not found, details: " + ex.Message);
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"Error generic get all, details error: {ex.Message}");
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
                    try
                    { 
                        listEpisodesAdd = episodeApi.PostMore("/episodes", listEpisodesAdd).GetAwaiter().GetResult();
                    }
                    catch (ApiGenericException ex)
                    {
                        _logger.Fatal($"Error generic post episodes, details error: {ex.Message}");
                    }

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

                    try
                    {
                        episodeRegisterApi.PostMore("/episodes/registers", listEpisodeRegister).GetAwaiter();
                    }
                    catch (ApiGenericException ex)
                    {
                        _logger.Fatal($"Error generic post registers, details error: {ex.Message}");
                    }

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

            _logger.Info($"End upgrade anime");
        }
    }
}
