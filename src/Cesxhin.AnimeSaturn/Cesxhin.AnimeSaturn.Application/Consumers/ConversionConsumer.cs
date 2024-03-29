﻿using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.DTO;
using FFMpegCore;
using FFMpegCore.Enums;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Consumers
{
    public class ConversionConsumer : IConsumer<ConversionDTO>
    {
        //nlog
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());
        
        //temp
        private string pathTemp = Environment.GetEnvironmentVariable("PATH_TEMP");
        public Task Consume(ConsumeContext<ConversionDTO> context)
        {
            try
            {
                var message = context.Message;

                Api<EpisodeDTO> episodeApi = new();
                Api<EpisodeRegisterDTO> episodeRegisterApi = new();

                EpisodeDTO episode = null;
                EpisodeRegisterDTO episodeRegister = null;
                //episode
                try
                {
                    episode = episodeApi.GetOne($"/episode/id/{message.ID}").GetAwaiter().GetResult();
                }
                catch (ApiNotFoundException ex)
                {
                    _logger.Error($"Not found episodeRegister, details error: {ex.Message}");
                }
                catch (ApiGenericException ex)
                {
                    _logger.Fatal($"Impossible error generic get episodeRegister, details error: {ex.Message}");
                
                }

                //episodeRegister
                try
                {
                    episodeRegister = episodeRegisterApi.GetOne($"/episode/register/episodeid/{episode.ID}").GetAwaiter().GetResult();
                }
                catch (ApiNotFoundException ex)
                {
                    _logger.Error($"Not found episodeRegister, details error: {ex.Message}");
                }
                catch (ApiGenericException ex)
                {
                    _logger.Fatal($"Impossible error generic get episodeRegister, details error: {ex.Message}");
                }

                //check
                if (episode == null)
                {
                    _logger.Fatal($"Get episode ID: {message.ID} not exitis");
                    return null;
                }

                var fileTemp = $"{pathTemp}/joined-{Path.GetFileName(message.FilePath)}.ts";

                if (!File.Exists(fileTemp))
                {
                    //read all bytes
                    List<byte[]> buffer = new();
                    foreach (var path in message.Paths)
                    {
                        buffer.Add(File.ReadAllBytes(path));
                    }

                    //join bytes
                    using (var file = new FileStream(fileTemp, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        foreach (var data in buffer)
                            file.Write(data);
                    }

                    //destroy
                    buffer.Clear();

                    //delete old files
                    foreach (var path in message.Paths)
                        File.Delete(path);
                }

                //send status api
                episode.StateDownload = "conversioning";
                SendStatusDownloadAPIAsync(episode, episodeApi);

                //convert ts to mp4
                var tempMp4 = $"{pathTemp}/{Path.GetFileName(message.FilePath)}";
                var process = FFMpegArguments
                    .FromFileInput(fileTemp)
                    .OutputToFile(tempMp4, true, options => options
                        .WithVideoCodec(VideoCodec.LibX264)
                        .WithAudioCodec(AudioCodec.Aac)
                        .WithVideoFilters(filterOptions => filterOptions
                            .Scale(VideoSize.FullHd)))
                    .ProcessSynchronously();

                File.Move(tempMp4, message.FilePath, true);

                //delete old file
                File.Delete(fileTemp);



                //get hash and update
                _logger.Info($"start calculate hash of episode id: {episode.ID}");
                string hash = Hash.GetHash(episodeRegister.EpisodePath);
                _logger.Info($"end calculate hash of episode id: {episode.ID}");

                episodeRegister.EpisodeHash = hash;

                try
                {
                    episodeRegisterApi.PutOne("/episode/register", episodeRegister).GetAwaiter().GetResult();
                }
                catch (ApiNotFoundException ex)
                {
                    _logger.Error($"Not found episodeRegister id: {episodeRegister.EpisodeId}, details error: {ex.Message}");
                }
                catch (ApiGenericException ex)
                {
                    _logger.Fatal($"Error generic put episodeRegister, details error: {ex.Message}");
                }

                //send status api
                episode.StateDownload = "completed";
                SendStatusDownloadAPIAsync(episode, episodeApi);

                return Task.CompletedTask;
            }
            catch(Exception ex)
            {
                _logger.Error($"Error generic, details {ex.Message}");
            }

            return null;
        }

        private void SendStatusDownloadAPIAsync(EpisodeDTO episode, Api<EpisodeDTO> episodeApi)
        {
            try
            {
                episodeApi.PutOne("/anime/statusDownload", episode).GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found episode id: {episode.ID}, details: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Error($"Error generic api, details: {ex.Message}");
            }
        }
    }
}
