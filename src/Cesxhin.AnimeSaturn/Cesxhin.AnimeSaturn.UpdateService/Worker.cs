using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Application.Parallel;
using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
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
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";
        private readonly int _timeRefresh = int.Parse(Environment.GetEnvironmentVariable("TIME_REFRESH") ?? "120000");

        public Worker(IBus publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //client
            HttpClient client = new();

            //Istance Api
            Api<GenericDTO> animeApi = new();
            Api<EpisodeDTO> episodeApi = new();
            Api<EpisodeRegisterDTO> episodeRegisterApi = new();

            //Instance Parallel
            ParallelManager<object> parallel = new();

            while (!stoppingToken.IsCancellationRequested)
            {
                //download api
                List<GenericDTO> listAnime = null;
                parallel.ClearList();

                try
                {
                    listAnime = await animeApi.GetMore("/all");
                }catch (ApiNotFoundException ex)
                {
                    _logger.Error($"Not found get all, details error: {ex.Message}");
                }catch(ApiGenericException ex)
                {
                    _logger.Fatal($"Error generic get all, details error: {ex.Message}");
                }

                //if exists listAnime
                if(listAnime != null)
                {
                    //step one check file
                    foreach (var anime in listAnime)
                    {
                        //foreach episodes
                        foreach (var episode in anime.Episodes)
                        {
                            parallel.AddTask(new Func<object>(() => CheckEpisode(anime, episode, episodeApi, episodeRegisterApi)));
                        }
                    }
                }

                _logger.Info($"Worker running at: {DateTimeOffset.Now}");
                await Task.Delay(_timeRefresh, stoppingToken);
            }
        }

        private object CheckEpisode(GenericDTO anime, EpisodeDTO episode, Api<EpisodeDTO> episodeApi, Api<EpisodeRegisterDTO> episodeRegisterApi)
        {
            var episodeRegister = anime.EpisodeRegister.Find(e => e.EpisodeId == episode.ID);
            if (episodeRegister == null)
            {
                _logger.Warn($"not found episodeRegister by episode id: {episode.ID}");
                return null;
            }

            _logger.Debug($"check {episodeRegister.EpisodePath}");

            //check integry file
            if (episode.StateDownload == null || episode.StateDownload == "failed" || (episode.StateDownload == "completed" && episodeRegister.EpisodeHash == null))
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
                        _logger.Info($"I found file (episode id: {episode.ID}) that was move, now update information");

                        //update
                        episodeRegister.EpisodePath = file;
                        try
                        {
                            episodeRegisterApi.PutOne("/episode/register", episodeRegister).GetAwaiter().GetResult();

                            _logger.Info($"Ok update episode id: {episode.ID} that was move");

                            //return
                            found = true;
                        }
                        catch (ApiNotFoundException ex)
                        {
                            _logger.Error($"Not found episodeRegister id: {episodeRegister.EpisodeId} for update information, details: {ex.Message}");
                        }
                        catch (ApiConflictException ex)
                        {
                            _logger.Error($"Error conflict put episodeRegister, details error: {ex.Message}");
                        }
                        catch (ApiGenericException ex)
                        {
                            _logger.Fatal($"Error generic put episodeRegister, details error: {ex.Message}");
                        }

                        break;
                    }
                }

                //if not found file
                if (found == false)
                    ConfirmStartDownloadAnime(episode, episodeApi);
            }

            return null;
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
                _logger.Info($"this file ({episode.AnimeId} episode: {episode.NumberEpisodeCurrent}) does not exists, sending message to DownloadService");
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Impossible update episode becouse not found episode id: {episode.ID}, details: {ex.Message}");
            }
            catch(ApiGenericException ex)
            {
                _logger.Fatal($"Error update episode, details error: {ex.Message}");
            }
        }
    }
}
