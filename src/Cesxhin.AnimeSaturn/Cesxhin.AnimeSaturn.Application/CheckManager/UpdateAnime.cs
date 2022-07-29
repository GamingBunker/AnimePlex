using Cesxhin.AnimeSaturn.Application.CheckManager.Interfaces;
using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Application.Parallel;
using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cesxhin.AnimeSaturn.Application.CheckManager
{
    public class UpdateAnime : IUpdate
    {
        //interface
        private readonly IBus _publishEndpoint;

        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";

        //Instance Parallel
        private readonly ParallelManager<object> parallel = new();

        //Istance Api
        private readonly Api<GenericAnimeDTO> animeApi = new();
        private readonly Api<EpisodeDTO> episodeApi = new();
        private readonly Api<EpisodeRegisterDTO> episodeRegisterApi = new();

        //download api
        private List<GenericAnimeDTO> listAnime = null;

        public UpdateAnime(IBus publicEndpoint)
        {
            _publishEndpoint = publicEndpoint;
        }

        public void ExecuteUpdate()
        {
            _logger.Info($"Start update anime");

            try
            {
                listAnime = animeApi.GetMore("/anime/all").GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found get all, details error: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"Error generic get all, details error: {ex.Message}");
            }

            //if exists listAnime
            if (listAnime != null)
            {
                var tasks = new List<Func<object>>();
                //step one check file
                foreach (var anime in listAnime)
                {

                    //foreach episodes
                    foreach (var episode in anime.Episodes)
                    {
                        tasks.Add(new Func<object>(() => CheckEpisode(anime, episode, episodeApi, episodeRegisterApi)));
                    }
                }
                parallel.AddTasks(tasks);
                parallel.Start();
                parallel.WhenCompleted();
                parallel.ClearList();
            }

            _logger.Info($"End update anime");
        }

        private object CheckEpisode(GenericAnimeDTO anime, EpisodeDTO episode, Api<EpisodeDTO> episodeApi, Api<EpisodeRegisterDTO> episodeRegisterApi)
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
            else if (episode.StateDownload == "completed")
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
                await episodeApi.PutOne("/anime/statusDownload", episode);

                await _publishEndpoint.Publish(episode);
                _logger.Info($"this file ({episode.AnimeId} episode: {episode.NumberEpisodeCurrent}) does not exists, sending message to DownloadService");
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Impossible update episode becouse not found episode id: {episode.ID}, details: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"Error update episode, details error: {ex.Message}");
            }
        }
    }
}
