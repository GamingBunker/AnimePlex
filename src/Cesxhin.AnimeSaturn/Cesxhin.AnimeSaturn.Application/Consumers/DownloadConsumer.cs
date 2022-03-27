﻿using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Consumers
{
    public class DownloadConsumer : IConsumer<EpisodeDTO>
    {
        //nlog
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //set variable
        private string _folder = Environment.GetEnvironmentVariable("BASE_PATH");
        private readonly string _address = Environment.GetEnvironmentVariable("ADDRESS_API");
        private readonly string _port = Environment.GetEnvironmentVariable("PORT_API");
        private readonly string _protocol = Environment.GetEnvironmentVariable("PROTOCOL_API");

        //for api and service download file with fist method(url with file)
        HttpClient clientHttp = new HttpClient();

        private static void InitiateSSLTrust()
        {
            try
            {
                //Change SSL checks so that all checks pass
                ServicePointManager.ServerCertificateValidationCallback =
                   new RemoteCertificateValidationCallback(
                        delegate
                        { return true; }
                    );
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public Task Consume(ConsumeContext<EpisodeDTO> context)
        {
            //get body
            var episode = context.Message;

            //set number view
            string formatNumberView = "D2";
            if (episode.NumberEpisodeCurrent > 99)
                formatNumberView = "D3";
            else if (episode.NumberEpisodeCurrent > 999)
                formatNumberView = "D4";

            //set path
            string filePath = $"{_folder}/{episode.IDAnime}/Season {episode.NumberSeasonCurrent.ToString("D2")}/{episode.IDAnime} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString(formatNumberView)}.mp4";
            string directoryPath = $"{_folder}/{episode.IDAnime}/Season {episode.NumberSeasonCurrent.ToString("D2")}";

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
                    client.Headers.Add("Referer", episode.Referer);
                    logger.Info("try download: " + episode.UrlVideo);

                    //start download
                    client.Timeout = 60000; //? check
                    client.DownloadFileTaskAsync(new Uri(episode.UrlVideo), filePath).GetAwaiter().GetResult();
                }
            }
            else
            {
                //url stream
                Download(episode, filePath);
            }
            return Task.CompletedTask;
        }

        //download url with files stream
        private void Download(EpisodeDTO episode, string filePath)
        {
            //disable ssl
            InitiateSSLTrust();

            //timeout if not response one resource and close with status failed
            int timeout = 5;

            //create file and save to end operation
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                List<byte[]> buffer = new List<byte[]>();

                using (var client = new WebClient())
                {
                    logger.Info("start download " + episode.IDAnime + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent);

                    //change by pending to downloading
                    episode.StateDownload = "downloading";
                    SendStatusDownloadAPIAsync(episode);

                    int count = 0;
                    int percentual;
                    int lastTriggerTime = 0;
                    int intervalCheck;
                    for (int numberFrame=episode.startNumberBuffer; numberFrame<= episode.endNumberBuffer; numberFrame++)
                    {
                        //url frame
                        string url = $"{episode.BaseUrl}/{episode.Resolution}/{episode.Resolution}-{numberFrame.ToString("D3")}.ts";
                        Uri uri = new Uri(url);

                        //download frame
                        do
                        {
                            if(timeout == 0)
                            {
                                //send api failed download
                                episode.StateDownload = "failed";
                                SendStatusDownloadAPIAsync(episode);

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
                                break;
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                                timeout--;
                            }
                        } while (true);

                        count++;
                        percentual = (100 * count) / episode.endNumberBuffer;
                        logger.Debug("status download " + episode.IDAnime + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent + "status download: "+ percentual);

                        //send only one data every 3 seconds
                        intervalCheck = DateTime.Now.Second;
                        if (lastTriggerTime > intervalCheck)
                            lastTriggerTime = 3;

                        if (intervalCheck % 3 == 0 && (intervalCheck - lastTriggerTime) >= 3)
                        {
                            lastTriggerTime = DateTime.Now.Second;

                            //send status download
                            episode.PercentualDownload = percentual;
                            SendStatusDownloadAPIAsync(episode);
                        }
                    }

                    logger.Info("end download " + episode.IDAnime + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent);

                }

                logger.Info("start join most buffer" + episode.IDAnime + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent);
                foreach (var singleBuffer in buffer)
                {
                    fs.Write(singleBuffer);
                }
                logger.Info("end join most buffer" + episode.IDAnime + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent);

                //send end download
                episode.StateDownload = "completed";
                episode.PercentualDownload = 100;
                SendStatusDownloadAPIAsync(episode);

                return;
            }
        }

        private DownloadProgressChangedEventHandler client_DownloadProgressChanged(string filePath, EpisodeDTO episode)
        {
            //change by pending to downloading
            episode.StateDownload = "downloading";
            int lastTriggerTime = 0;
            int intervalCheck;

            Action<object, DownloadProgressChangedEventArgs> action = (sender, e) =>
            {
                //print progress
                logger.Debug(e.ProgressPercentage + "% | " + e.BytesReceived + " bytes out of " + e.TotalBytesToReceive + " bytes retrieven of the file: "+filePath);

                //send only one data every 3 seconds
                intervalCheck = DateTime.Now.Second;
                if (lastTriggerTime > intervalCheck)
                    lastTriggerTime = 3; 

                if (intervalCheck % 3 == 0 && (intervalCheck - lastTriggerTime) >= 3)
                {
                    lastTriggerTime = DateTime.Now.Second;

                    //send status download
                    episode.PercentualDownload = e.ProgressPercentage;
                    SendStatusDownloadAPIAsync(episode);
                }
            };
            return new DownloadProgressChangedEventHandler(action);

        }
        private AsyncCompletedEventHandler client_DownloadFileCompleted(string filePath, EpisodeDTO episode)
        {
            try
            {
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
                        SendStatusDownloadAPIAsync(episode);
                    }
                    else
                    {
                        logger.Info($"Download completed! {filePath}");

                        //download finish download
                        episode.StateDownload = "completed";
                        episode.PercentualDownload = 100;
                        SendStatusDownloadAPIAsync(episode);
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

        private void SendStatusDownloadAPIAsync(EpisodeDTO episode)
        {
            using (var clientAPI = new HttpClient())
            using (var content = new StringContent(JsonSerializer.Serialize(episode), System.Text.Encoding.UTF8, "application/json"))
            {
                var resultHttp = clientAPI.PutAsync($"{_protocol}://{_address}:{_port}/statusDownload", content).GetAwaiter().GetResult();
                if(resultHttp.IsSuccessStatusCode)
                {
                    logger.Debug("Confirm send to API: " + episode.IDAnime);
                }
                else
                {
                    logger.Error("Error send to API: " + episode.IDAnime+ " Details error: "+resultHttp.Content.ReadAsStreamAsync().GetAwaiter().GetResult());
                }
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
