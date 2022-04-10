using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.UpdateService
{
    public class Worker : BackgroundService
    {
        //interface
        private readonly IBus _publishEndpoint;

        //log
        private readonly NLogConsole logger = new NLogConsole(LogManager.GetCurrentClassLogger());

        //env
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";
        private readonly int _timeRefresh = int.Parse(Environment.GetEnvironmentVariable("TIME_REFRESH") ?? "120000");

        public Worker(IBus publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HttpClient client = new HttpClient();

            //Istance Api
            Api<GenericDTO> animeApi = new Api<GenericDTO>();
            Api<EpisodeDTO> episodeApi = new Api<EpisodeDTO>();

            while (!stoppingToken.IsCancellationRequested)
            {
                //download api
                var listAnime = await animeApi.GetMore("/all");

                if(listAnime != null)
                {
                    //step one check file
                    foreach (var anime in listAnime)
                    {
                        //foreach episodes
                        foreach (var episode in anime.Episodes)
                        {
                            var episodeRegister = anime.EpisodeRegister.Find(e => e.EpisodeId == episode.ID);
                            if (episodeRegister == null)
                                throw new Exception("not found episodeRegister");

                            logger.Debug($"check {episodeRegister.EpisodePath}");

                            //check integry file
                            if (episode.StateDownload == null || episode.StateDownload == "failed")
                            {
                                ConfirmStartDownloadAnime(episode, episodeApi);
                            }
                            else if ((!File.Exists(episodeRegister.EpisodePath) && episode.StateDownload != "pending"))
                            {
                                var found = false;
                                string newHash;
                                foreach (string file in Directory.EnumerateFiles(_folder, "*.mp4", SearchOption.AllDirectories))
                                {
                                    newHash = Hash.GetHash(file);
                                    if (newHash == episodeRegister.EpisodeHash)
                                    {
                                        logger.Info($"I found file (episode id: {episode.ID}) that was move, now update information");
                                        
                                        //api
                                        Api<EpisodeRegisterDTO> episodeRegisterApi = new Api<EpisodeRegisterDTO>();
                                        
                                        //update
                                        episodeRegister.EpisodePath = file;
                                        try
                                        {
                                            await episodeRegisterApi.PutOne("/episode/register", episodeRegister);
                                        
                                            logger.Info($"Ok update episode id: {episode.ID} that was move");
                                        
                                            //return
                                            found = true;
                                        }catch (ApiNotFoundException ex)
                                        {
                                            logger.Error($"Not found episodeRegister id: {episodeRegister.EpisodeId} for update information, details: {ex.Message}");
                                        }
                                        
                                        break;
                                    }
                                }

                                //if not found file
                                if(found == false)
                                    ConfirmStartDownloadAnime(episode, episodeApi);
                            }
                        }
                    }
                }

                logger.Info($"Worker running at: {DateTimeOffset.Now}");
                await Task.Delay(_timeRefresh, stoppingToken);
            }
        }

        private async void ConfirmStartDownloadAnime(EpisodeDTO episode, Api<EpisodeDTO> episodeApi)
        {
            //set pending to 
            episode.StateDownload = "pending";

            try
            {
                //set change status
                await episodeApi.PutOne("/statusDownload", episode);

                await _publishEndpoint.Publish(episode);
                logger.Info($"this file ({episode.AnimeId} episode: {episode.NumberEpisodeCurrent}) does not exists, sending message to DownloadService");
            }
            catch (ApiNotFoundException ex)
            {
                logger.Error($"Impossible update episode becouse not found episode id: {episode.ID}, details: {ex.Message}");
            }
        }
    }
}
