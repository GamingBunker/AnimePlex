using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Consumers
{
    public class DownloadConsumer : IConsumer<EpisodeDTO>
    {
        //const
        const int LIMIT_TIMEOUT = 10;

        //nlog
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Task Consume(ConsumeContext<EpisodeDTO> context)
        {
            //get body
            var episode = context.Message;

            //api
            Api<EpisodeRegisterDTO> episodeRegisterApi = new Api<EpisodeRegisterDTO>();
            Api<AnimeDTO> animeApi = new Api<AnimeDTO>();

            var episodeRegister = episodeRegisterApi.GetOne($"/episode/register/episodeid/{episode.ID}").GetAwaiter().GetResult();

            //paths
            var directoryPath = Path.GetDirectoryName(episodeRegister.EpisodePath);
            var filePath = episodeRegister.EpisodePath;

            //check directory
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            //check type url
            if (episode.UrlVideo != null)
            {
                //url with file
                using (var client = new MyWebClient())
                {
                    //task
                    client.DownloadProgressChanged += client_DownloadProgressChanged(filePath, episode);
                    client.DownloadFileCompleted += client_DownloadFileCompleted(filePath, episode);

                    //add referer for download, also recive error 403 forbidden
                    
                    logger.Info("try download: " + episode.UrlVideo);
                    try
                    {
                        var anime = animeApi.GetOne($"/anime/name/{episode.AnimeId}").GetAwaiter().GetResult();
                        //start download
                        client.Headers.Add("Referer", anime.UrlPage);
                        client.Timeout = 60000; //? check
                        client.DownloadFileTaskAsync(new Uri(episode.UrlVideo), filePath).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    catch (ApiNotFoundException ex)
                    {
                        logger.Error($"not found anime so can't set headers referer for download, details: {ex.Message}");
                    }
                }
            }
            else
            {
                //url stream
                Download(episode, filePath);
            }

            //get hash and update
            logger.Info($"start calculate hash of episode id: {episode.ID}");
            string hash = Hash.GetHash(episodeRegister.EpisodePath);
            logger.Info($"end calculate hash of episode id: {episode.ID}");

            episodeRegister.EpisodeHash = hash;

            try
            {
                episodeRegisterApi.PutOne("/episode/register", episodeRegister).GetAwaiter().GetResult();
            }catch (ApiNotFoundException ex)
            {
                logger.Error($"Not found episodeRegister id: {episodeRegister.ID}, details: {ex.Message}");
            }

            logger.Info($"Completed task download episode id: {episode.ID}");
            return Task.CompletedTask;
        }

        //download url with files stream
        private void Download(EpisodeDTO episode, string filePath)
        {
            //timeout if not response one resource and close with status failed
            int timeout = 0;
            int timeoutFile = 0;

            //api
            Api<EpisodeDTO> episodeDTO = new Api<EpisodeDTO>();

            while (true)
            {
                if (timeoutFile == LIMIT_TIMEOUT)
                {
                    //send api failed download
                    episode.StateDownload = "failed";
                    SendStatusDownloadAPIAsync(episode, episodeDTO);
                    throw new Exception($"{filePath} impossible open file, contact administrator please");
                }
                try
                {
                    //create file and save to end operation
                    using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        List<byte[]> buffer = new List<byte[]>();

                        using (var client = new MyWebClient())
                        {
                            client.Timeout =  60000; //? check
                            logger.Info("start download " + episode.AnimeId + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent);

                            //change by pending to downloading
                            episode.StateDownload = "downloading";
                            SendStatusDownloadAPIAsync(episode, episodeDTO);

                            int count = 0;
                            int percentual;
                            int lastTriggerTime = 0;
                            int intervalCheck;
                            for (int numberFrame = episode.startNumberBuffer; numberFrame <= episode.endNumberBuffer; numberFrame++)
                            {
                                //url frame
                                string url = $"{episode.BaseUrl}/{episode.Resolution}/{episode.Resolution}-{numberFrame.ToString("D3")}.ts";
                                Uri uri = new Uri(url);

                                //download frame
                                do
                                {
                                    if (timeout == LIMIT_TIMEOUT)
                                    {
                                        //send api failed download
                                        episode.StateDownload = "failed";
                                        SendStatusDownloadAPIAsync(episode, episodeDTO);

                                        logger.Error("Failed download, details: " + url);

                                        //delete file
                                        fs.Close();
                                        if (File.Exists(filePath))
                                        {
                                            File.Delete(filePath);
                                            logger.Warn($"The file is deleted {filePath}");
                                        }
                                        return;
                                    }
                                    try
                                    {
                                        buffer.Add(client.DownloadData(uri));
                                        timeout = 0;
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.Error(ex);
                                        logger.Warn($"The attempts remains: {LIMIT_TIMEOUT - timeout} for {url}");
                                        timeout++;

                                        //waiting before for re-download
                                        Thread.Sleep(timeout * 1000);
                                    }
                                } while (true);

                                count++;
                                percentual = (100 * count) / episode.endNumberBuffer;
                                logger.Debug("status download " + episode.AnimeId + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent + "status download: " + percentual + "%");

                                //send only one data every 3 seconds
                                intervalCheck = DateTime.Now.Second;
                                if (lastTriggerTime > intervalCheck)
                                    lastTriggerTime = 3;

                                if (intervalCheck % 3 == 0 && (intervalCheck - lastTriggerTime) >= 3)
                                {
                                    lastTriggerTime = DateTime.Now.Second;

                                    //send status download
                                    episode.PercentualDownload = percentual;
                                    SendStatusDownloadAPIAsync(episode, episodeDTO);
                                }
                            }
                            logger.Info("end download " + episode.AnimeId + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent);

                        }

                        logger.Info("start join most buffer" + episode.AnimeId + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent);

                        foreach (var singleBuffer in buffer)
                        {
                            fs.Write(singleBuffer);
                        }

                        logger.Info("end join most buffer" + episode.AnimeId + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent);

                        //send end download
                        episode.StateDownload = "completed";
                        episode.PercentualDownload = 100;
                        SendStatusDownloadAPIAsync(episode, episodeDTO);
                    }
                    return;
                }
                catch (IOException ex)
                {
                    logger.Error($"{filePath} can't open, details: {ex.Message}");
                    timeoutFile++;
                }
            }
        }

        private DownloadProgressChangedEventHandler client_DownloadProgressChanged(string filePath, EpisodeDTO episode)
        {
            //change by pending to downloading
            episode.StateDownload = "downloading";

            int lastTriggerTime = 0;
            int intervalCheck;

            //api
            Api<EpisodeDTO> episodeDTO = new Api<EpisodeDTO>();

            try
            {
                Action<object, DownloadProgressChangedEventArgs> action = (sender, e) =>
                {
                    //print progress
                    logger.Debug(e.ProgressPercentage + "% | " + e.BytesReceived + " bytes out of " + e.TotalBytesToReceive + " bytes retrieven of the file: " + filePath);

                    //send only one data every 3 seconds
                    intervalCheck = DateTime.Now.Second;
                    if (lastTriggerTime > intervalCheck)
                        lastTriggerTime = 3;

                    if (intervalCheck % 3 == 0 && (intervalCheck - lastTriggerTime) >= 3)
                    {
                        lastTriggerTime = DateTime.Now.Second;

                        //send status download
                        episode.PercentualDownload = e.ProgressPercentage;
                        SendStatusDownloadAPIAsync(episode, episodeDTO);
                    }
                };
                return new DownloadProgressChangedEventHandler(action);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            return null;

        }
        private AsyncCompletedEventHandler client_DownloadFileCompleted(string filePath, EpisodeDTO episode)
        {
            try
            {
                //api
                Api<EpisodeDTO> episodeDTO = new Api<EpisodeDTO>();

                //recive response action
                Action<object, AsyncCompletedEventArgs> action = (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        logger.Error($"Interrupt download file {filePath}");
                        logger.Error(e.Error);

                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            logger.Warn($"The file is deleted {filePath}");
                        }

                        //send failed download
                        episode.StateDownload = "failed";
                        SendStatusDownloadAPIAsync(episode, episodeDTO);
                    }
                    else
                    {
                        logger.Info($"Download completed! {filePath}");

                        //download finish download
                        episode.StateDownload = "completed";
                        episode.PercentualDownload = 100;
                        SendStatusDownloadAPIAsync(episode, episodeDTO);
                    }
                };
                return new AsyncCompletedEventHandler(action);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return null;
        }

        private void SendStatusDownloadAPIAsync(EpisodeDTO episode, Api<EpisodeDTO> episodeApi)
        {
            try
            {
                episodeApi.PutOne("/statusDownload", episode).GetAwaiter().GetResult();
            }catch (ApiNotFoundException ex)
            {
                logger.Error($"Not found episode id: {episode.ID}, details: {ex.Message}");
            }catch (ApiGenericException ex)
            {
                logger.Error($"Error generic api, details: {ex.Message}");
            }
        }
    }

    //custom WebClient for set Timeout
    public class MyWebClient : WebClient
    {
        public int? Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);
            webRequest.Timeout = (int)Timeout;
            return webRequest;
        }
    }
}
