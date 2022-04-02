using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.HtmlAgilityPack;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.UpgradeService
{
    public class Worker : BackgroundService
    {
        //log
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //variables
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH");
        private readonly int _timeRefresh = int.Parse(Environment.GetEnvironmentVariable("TIME_REFRESH"));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                List<GenericDTO> listGenerics = new List<GenericDTO>();
                List<EpisodeRegisterDTO> listEpisodeRegister;
                List<EpisodeRegisterDTO> blacklist;

                //api
                Api<GenericDTO> genericApi = new Api<GenericDTO>();
                Api<EpisodeDTO> episodeApi = new Api<EpisodeDTO>();
                Api<EpisodeRegisterDTO> episodeRegisterApi = new Api<EpisodeRegisterDTO>();

                try
                {
                    listGenerics = await genericApi.GetMore("/all");
                }catch (ApiNotFoundException ex)
                {
                    logger.Error($"not found, details: "+ex.Message);
                }

                //step check on website if the anime is still active
                foreach (var list in listGenerics)
                {
                    var anime = list.Anime;

                    //get list episodes by name
                    List<EpisodeDTO> checkEpisodes = null;
                    List<EpisodeDTO> listEpisodesAdd = null;

                    logger.Info("Check new episodes for Anime: " + anime.Name);

                    //check new episode
                    checkEpisodes = HtmlAnimeSaturn.GetEpisodes(anime.UrlPage, anime.Name);

                    //check if null
                    if (checkEpisodes == null)
                    {
                        logger.Error($"Can't download with this url, {anime.UrlPage}");
                        continue;
                    }

                    listEpisodesAdd = new List<EpisodeDTO>(checkEpisodes);
                    blacklist = new List<EpisodeRegisterDTO>();

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
                        logger.Info($"There are new episodes ({listEpisodesAdd.Count}) of {anime.Name}");

                        //insert to db
                        listEpisodesAdd = await episodeApi.PostMore("/episodes", listEpisodesAdd);

                        //create episodeRegister
                        listEpisodeRegister = new List<EpisodeRegisterDTO>();

                        string formatNumberView;
                        string pathDefault = null; 
                        string path = null;

                        if (blacklist.Count > 0)
                            pathDefault = Path.GetDirectoryName(blacklist.FirstOrDefault().EpisodePath);

                        foreach (var episode in listEpisodesAdd)
                        {
                            formatNumberView = "D2"; //default
                                                     //check max space numbers
                            if (episode.NumberEpisodeCurrent > 99)
                                formatNumberView = "D3";
                            else if (episode.NumberEpisodeCurrent > 999)
                                formatNumberView = "D4";

                            path = "";
                            //use path how others episodesRegisters
                            if (pathDefault != null)
                            {
                                path = $"{pathDefault}/{episode.AnimeId} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString(formatNumberView)}.mp4";
                            }
                            else //default
                            {
                                path = $"{_folder}/{episode.AnimeId}/Season {episode.NumberSeasonCurrent.ToString("D2")}/{episode.AnimeId} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString(formatNumberView)}.mp4";
                            }

                            listEpisodeRegister.Add(new EpisodeRegisterDTO
                            {
                                EpisodeId = episode.ID,
                                EpisodePath = path
                            });
                        }

                        await episodeRegisterApi.PostMore("/episodes/registers", listEpisodeRegister);

                        logger.Info($"Done upgrade! of {anime.Name}");
                    }
                    //clear resource
                    listEpisodesAdd.Clear();
                }

                logger.Debug("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(_timeRefresh, stoppingToken);
            }
        }
    }
}
