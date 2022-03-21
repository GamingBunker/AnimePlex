using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using NLog;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Consumers
{
    public class DownloadConsumer : IConsumer<EpisodeDTO>
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string _folder = Environment.GetEnvironmentVariable("BASE_PATH");

        public Task Consume(ConsumeContext<EpisodeDTO> context)
        {
            //get body
            var episode = context.Message;

            //split url
            string[] path = episode.UrlVideo.Split('/');

            //set path
            string filePath = $"{_folder}/{episode.IDAnime}/{path[path.Length - 1]}";
            string directoryPath = $"{_folder}/{episode.IDAnime}";

            //check
            if (!File.Exists(filePath))
            {
                using (var client = new WebClient())
                {
                    //task
                    client.DownloadProgressChanged += client_DownloadProgressChanged(filePath);
                    client.DownloadFileCompleted += client_DownloadFileCompleted(filePath);

                    //add referer for download, also recive error 403 forbidden
                    client.Headers.Add("Referer", episode.Referer);
                    logger.Info("try download: " + episode.UrlVideo);

                    //check directory
                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    //check file
                    if (File.Exists(filePath))
                    {
                        logger.Info("it already exists");
                        return Task.CompletedTask;
                    }

                    //start download
                    client.DownloadFileAsync(new Uri(episode.UrlVideo), filePath);
                }
            }
            return Task.CompletedTask;
        }

        private DownloadProgressChangedEventHandler client_DownloadProgressChanged(string filePath)
        {
            Action<object, DownloadProgressChangedEventArgs> action = (sender, e) =>
            {
                //print progress
                logger.Debug(e.ProgressPercentage + "% | " + e.BytesReceived + " bytes out of " + e.TotalBytesToReceive + " bytes retrieven of the file: "+filePath);
            };
            return new DownloadProgressChangedEventHandler(action);

        }
        private AsyncCompletedEventHandler client_DownloadFileCompleted(string filePath)
        {
            //recive response action
            Action<object, AsyncCompletedEventArgs> action = (sender, e) =>
            {
                if (e.Error != null)
                {
                    logger.Error($"Interrupt download file {filePath}");

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        logger.Warn($"The file was deleted {filePath}");
                    }
                }else
                    logger.Info($"Download completed! {filePath}");
            };
            return new AsyncCompletedEventHandler(action);
        }
    }
}
