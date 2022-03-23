using Cesxhin.AnimeSaturn.Domain.DTO;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //set variable
        private string _folder = Environment.GetEnvironmentVariable("BASE_PATH");
        private readonly string _address = Environment.GetEnvironmentVariable("ADDRESS_API");
        private readonly string _port = Environment.GetEnvironmentVariable("PORT_API");
        private readonly string _protocol = Environment.GetEnvironmentVariable("PROTOCOL_API");

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

            //set path
            string filePath = $"{_folder}/{episode.IDAnime}/Season {episode.NumberSeasonCurrent.ToString("D2")}/{episode.IDAnime} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString("D2")}.mp4";
            string directoryPath = $"{_folder}/{episode.IDAnime}/Season {episode.NumberSeasonCurrent.ToString("D2")}";

            //check directory
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            if (episode.UrlVideo != null)
            {
                //url traditional
                using (var client = new MyWebClient())
                {
                    //task
                    client.DownloadProgressChanged += client_DownloadProgressChanged(filePath, episode);
                    client.DownloadFileCompleted += client_DownloadFileCompleted(filePath, episode);

                    //add referer for download, also recive error 403 forbidden
                    client.Headers.Add("Referer", episode.Referer);
                    logger.Info("try download: " + episode.UrlVideo);

                    //start download
                    client.Timeout = 60000;
                    client.DownloadFileTaskAsync(new Uri(episode.UrlVideo), filePath).GetAwaiter().GetResult();
                }
            }
            else
            {
                //new method download file
                Download(episode, filePath);
            }
            return Task.CompletedTask;
        }

        private async void Download(EpisodeDTO episode, string filePath)
        {
            InitiateSSLTrust();
            HttpClient clientHttp = new HttpClient();
            int timeout = 5;

            //save to file
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {

                List<byte[]> buffer = new List<byte[]>();
                using (var client = new WebClient())
                {
                    logger.Info("start download " + episode.IDAnime + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent);

                    //send api start download
                    episode.StateDownload = "downloading";
                    using (var content = new StringContent(JsonSerializer.Serialize(episode), System.Text.Encoding.UTF8, "application/json"))
                    {
                        clientHttp.PutAsync($"{_protocol}://{_address}:{_port}/statusDownload", content).GetAwaiter().GetResult();
                    }

                    int count = 0;
                    int percentual;
                    foreach (var source in episode.Sources)
                    {
                        Uri uri = new Uri(episode.BaseUrl + "/" +episode.Resolution + "/" + source);
                        do
                        {
                            if(timeout == 0)
                            {
                                //send api failed download
                                episode.StateDownload = "failed";
                                using (var content = new StringContent(JsonSerializer.Serialize(episode), System.Text.Encoding.UTF8, "application/json"))
                                {
                                    clientHttp.PutAsync($"{_protocol}://{_address}:{_port}/statusDownload", content).GetAwaiter().GetResult();
                                }
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
                        percentual = (100 * count) / episode.Sources.Length;
                        logger.Debug("status download " + episode.IDAnime + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent + "status download: "+ percentual);

                        //send status download
                        if (DateTime.Now.Second % 3 == 0)
                        {
                            episode.PercentualDownload = percentual;
                            using (var content = new StringContent(JsonSerializer.Serialize(episode), System.Text.Encoding.UTF8, "application/json"))
                            {
                                clientHttp.PutAsync($"{_protocol}://{_address}:{_port}/statusDownload", content).GetAwaiter().GetResult();
                            }
                        }
                    }

                    logger.Info("end download " + episode.IDAnime + "s" + episode.NumberSeasonCurrent + "-e" + episode.NumberEpisodeCurrent);

                }

                foreach(var singleBuffer in buffer)
                {
                    fs.Write(singleBuffer);
                    logger.Warn("join singlebuffer to one big buffer");
                }

                //send end download
                episode.StateDownload = "completed";
                using (var content = new StringContent(JsonSerializer.Serialize(episode), System.Text.Encoding.UTF8, "application/json"))
                {
                    clientHttp.PutAsync($"{_protocol}://{_address}:{_port}/statusDownload", content).GetAwaiter().GetResult();
                }
            }
        }

        private DownloadProgressChangedEventHandler client_DownloadProgressChanged(string filePath, EpisodeDTO episode)
        {
            HttpClient clientHttp = new HttpClient();

            episode.StateDownload = "downloading";

            Action<object, DownloadProgressChangedEventArgs> action = (sender, e) =>
            {
                //print progress
                logger.Debug(e.ProgressPercentage + "% | " + e.BytesReceived + " bytes out of " + e.TotalBytesToReceive + " bytes retrieven of the file: "+filePath);

                //send status download
                if (DateTime.Now.Second % 3 == 0)
                {
                    episode.PercentualDownload = e.ProgressPercentage;
                    using (var content = new StringContent(JsonSerializer.Serialize(episode), System.Text.Encoding.UTF8, "application/json"))
                    {
                        clientHttp.PutAsync($"{_protocol}://{_address}:{_port}/statusDownload", content).GetAwaiter().GetResult();
                    }
                }
            };
            return new DownloadProgressChangedEventHandler(action);

        }
        private AsyncCompletedEventHandler client_DownloadFileCompleted(string filePath, EpisodeDTO episode)
        {
            HttpClient clientHttp = new HttpClient();
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
                        using (var content = new StringContent(JsonSerializer.Serialize(episode), System.Text.Encoding.UTF8, "application/json"))
                        {
                            clientHttp.PutAsync($"{_protocol}://{_address}:{_port}/statusDownload", content).GetAwaiter().GetResult();
                        }
                    }
                    else
                    {
                        logger.Info($"Download completed! {filePath}");

                        //download finish download
                        episode.StateDownload = "completed";
                        using (var content = new StringContent(JsonSerializer.Serialize(episode), System.Text.Encoding.UTF8, "application/json"))
                        {
                            clientHttp.PutAsync($"{_protocol}://{_address}:{_port}/statusDownload", content).GetAwaiter().GetResult();
                        }
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
    }
    public class MyWebClient : WebClient
    {
        public int? Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest webRequest = base.GetWebRequest(uri);
            if (this.Timeout.HasValue)
            {
                webRequest.Timeout = (int)Timeout;
            }
            return webRequest;
        }
    }
}
