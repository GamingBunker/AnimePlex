using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Application.Parallel;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
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
        private static readonly int NUMBER_PARALLEL_MAX = int.Parse(Environment.GetEnvironmentVariable("LIMIT_THREAD_PARALLEL") ?? "250");

        //nlog
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());


        public Task Consume(ConsumeContext<EpisodeDTO> context)
        {
            //get body
            var episode = context.Message;

            //api
            Api<EpisodeRegisterDTO> episodeRegisterApi = new();
            Api<AnimeDTO> animeApi = new();
            Api<EpisodeDTO> episodeApi = new();

            EpisodeRegisterDTO episodeRegister = null;
            EpisodeDTO episodeVerify = null;

            //episodeRegister
            try
            {
                episodeRegister = episodeRegisterApi.GetOne($"/episode/register/episodeid/{episode.ID}").GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found episodeRegister, details error: {ex.Message}");
            }catch (ApiGenericException ex)
            {
                _logger.Fatal($"Impossible error generic get episodeRegister, details error: {ex.Message}");
            }

            //episode
            try
            {
                episodeVerify = episodeApi.GetOne($"/episode/id/{episode.ID}").GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found episodeRegister, details error: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"Impossible error generic get episodeRegister, details error: {ex.Message}");
            }

            //check duplication messages
            if (episodeVerify != null && episodeVerify.StateDownload == "pending" )
            {
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

                        _logger.Info("try download: " + episode.UrlVideo);
                        try
                        {
                            var anime = animeApi.GetOne($"/anime/name/{episode.AnimeId}").GetAwaiter().GetResult();

                            //setup client
                            client.Headers.Add("Referer", anime.UrlPage);
                            client.Timeout = 60000; //? check

                            //start download
                            client.DownloadFileTaskAsync(new Uri(episode.UrlVideo), filePath).ConfigureAwait(false).GetAwaiter().GetResult();
                        }
                        catch (ApiNotFoundException ex)
                        {
                            _logger.Error($"not found anime so can't set headers referer for download, details: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            _logger.Fatal($"Error download with url easy, details error: {ex.Message}");
                        }
                    }
                }
                else
                {
                    //url stream
                    try
                    {
                        Download(episode, filePath);
                    }catch (Exception ex)
                    {
                        _logger.Fatal($"Error download with url stream, details error: {ex.Message}");
                    }
                }

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
            }

            _logger.Info($"Completed task download episode id: {episode.ID}");
            return Task.CompletedTask;
        }

        //download url with files stream
        private void Download(EpisodeDTO episode, string filePath)
        {
            //timeout if not response one resource and close with status failed
            int timeoutFile = 0;

            //api
            Api<EpisodeDTO> episodeDTOApi = new();

            while (true)
            {
                if (timeoutFile == LIMIT_TIMEOUT)
                {
                    //send api failed download
                    episode.StateDownload = "failed";
                    SendStatusDownloadAPIAsync(episode, episodeDTOApi);

                    throw new Exception($"{filePath} impossible open file, contact administrator please");
                }
                try
                {
                    //create file and save to end operation
                    using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        List<EpisodeBuffer> buffer = new();

                        _logger.Info($"start download {episode.AnimeId} s{episode.NumberSeasonCurrent}-e{episode.NumberEpisodeCurrent}");

                        //change by pending to downloading
                        episode.StateDownload = "downloading";
                        SendStatusDownloadAPIAsync(episode, episodeDTOApi);


                        //parallel
                        ParallelManager<EpisodeBuffer> parallel = new();

                        for (int numberFrame = episode.startNumberBuffer; numberFrame < episode.endNumberBuffer; numberFrame++)
                        {
                            var numberFrameSave = numberFrame;
                            parallel.AddTask(new Func<EpisodeBuffer>(() => { return DownloadBuffParallel(episode, numberFrameSave, filePath, episodeDTOApi); }));
                        }

                        while(!parallel.CheckFinish())
                        {
                            //send status download
                            episode.PercentualDownload = parallel.PercentualCompleted();
                            SendStatusDownloadAPIAsync(episode, episodeDTOApi);
                            Thread.Sleep(3000);
                        }

                        buffer = parallel.GetResult();

                        if(buffer == null)
                        {
                            //send end download
                            episode.StateDownload = "failed";
                            episode.PercentualDownload = 0;
                            SendStatusDownloadAPIAsync(episode, episodeDTOApi);

                            _logger.Error($"failed download {episode.AnimeId} s{episode.NumberSeasonCurrent}-e{episode.NumberEpisodeCurrent}");
                            return;
                        }

                        _logger.Info($"end download {episode.AnimeId} s{episode.NumberSeasonCurrent}-e{episode.NumberEpisodeCurrent}");

                        _logger.Info($"start join most buffer {episode.AnimeId} s{episode.NumberSeasonCurrent}-e{episode.NumberEpisodeCurrent}");

                        //order by id
                        buffer.Sort(delegate (EpisodeBuffer e1, EpisodeBuffer e2) { return e1.Id.CompareTo(e2.Id); });

                        foreach (var singleBuffer in buffer)
                        {
                            fs.Write(singleBuffer.Data);
                        }

                        _logger.Info($"end join most buffer {episode.AnimeId} s{episode.NumberSeasonCurrent}-e{episode.NumberEpisodeCurrent}");

                        //send end download
                        episode.StateDownload = "completed";
                        episode.PercentualDownload = 100;
                        SendStatusDownloadAPIAsync(episode, episodeDTOApi);
                    }
                    return;
                }
                catch (IOException ex)
                {
                    _logger.Error($"{filePath} can't open, details: {ex.Message}");
                    timeoutFile++;
                }
                catch (Exception ex)
                {
                    _logger.Fatal($"{filePath} can't open, details: {ex.Message}");
                }
            }
        }

        private EpisodeBuffer DownloadBuffParallel(EpisodeDTO episode, int numberFrame, string filePath, Api<EpisodeDTO> episodeDTOApi)
        {
            string url = $"{episode.BaseUrl}/{episode.Resolution}/{episode.Resolution}-{numberFrame.ToString("D3")}.ts";
            Uri uri = new Uri(url);

            //setup
            int timeout = 0;

            //download frame
            using (var client = new MyWebClient())
            {
                client.Timeout = 60000; //? check

                do
                {
                    if (timeout == LIMIT_TIMEOUT)
                    {
                        //send api failed download
                        episode.StateDownload = "failed";
                        SendStatusDownloadAPIAsync(episode, episodeDTOApi);

                        _logger.Error($"Failed download, details: {url}");

                        //delete file
                        //fs.Close();
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                            _logger.Warn($"The file is deleted {filePath}");
                        }
                        return null;
                    }
                    try
                    {
                        var data = client.DownloadData(uri);
                        return new EpisodeBuffer
                        {
                            Id = numberFrame,
                            Data = data,
                        };
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                        _logger.Warn($"The attempts remains: {LIMIT_TIMEOUT - timeout} for {url}");
                        timeout++;

                        //waiting before for re-download
                        Thread.Sleep(timeout * 1000);
                    }
                } while (true);
            }
        }

        private DownloadProgressChangedEventHandler client_DownloadProgressChanged(string filePath, EpisodeDTO episode)
        {
            //change by pending to downloading
            episode.StateDownload = "downloading";

            int lastTriggerTime = 0;
            int intervalCheck;

            //api
            Api<EpisodeDTO> episodeDTO = new();

            try
            {
                Action<object, DownloadProgressChangedEventArgs> action = (sender, e) =>
                {
                    //print progress
                    _logger.Debug(e.ProgressPercentage + "% | " + e.BytesReceived + " bytes out of " + e.TotalBytesToReceive + " bytes retrieven of the file: " + filePath);

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
                _logger.Error(ex.Message);
            }
            return null;

        }
        private AsyncCompletedEventHandler client_DownloadFileCompleted(string filePath, EpisodeDTO episode)
        {
            try
            {
                //api
                Api<EpisodeDTO> episodeDTO = new();

                //recive response action
                Action<object, AsyncCompletedEventArgs> action = (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        _logger.Error($"Interrupt download file {filePath}");
                        _logger.Error(e.Error);

                        if (File.Exists(filePath))
                        {
                            try
                            {
                                File.Delete(filePath);
                                _logger.Warn($"The file is deleted {filePath}");
                            }
                            catch (IOException ex)
                            {
                                _logger.Error($"cannot delete file {filePath}, details error:{ex.Message}");
                            }
                        }

                        //send failed download
                        episode.StateDownload = "failed";
                        SendStatusDownloadAPIAsync(episode, episodeDTO);
                    }
                    else
                    {
                        _logger.Info($"Download completed! {filePath}");

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
                _logger.Error(ex);
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
                _logger.Error($"Not found episode id: {episode.ID}, details: {ex.Message}");
            }catch (ApiGenericException ex)
            {
                _logger.Error($"Error generic api, details: {ex.Message}");
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
