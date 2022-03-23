using Cesxhin.AnimeSaturn.Application.HtmlAgilityPack;
using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.UpdateService
{
    public class Worker : BackgroundService
    {
        //interface
        private readonly IBus _publishEndpoint;

        //variables
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH");
        private readonly string _address = Environment.GetEnvironmentVariable("ADDRESS_API");
        private readonly string _port = Environment.GetEnvironmentVariable("PORT_API");
        private readonly string _protocol = Environment.GetEnvironmentVariable("PROTOCOL_API");
        private readonly int _timeRefresh = int.Parse(Environment.GetEnvironmentVariable("TIME_REFRESH"));

        public Worker(IBus publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            HttpClient client = new HttpClient();
            while (!stoppingToken.IsCancellationRequested)
            {
                //download api
                var resultAnime = await client.GetStringAsync($"{_protocol}://{_address}:{_port}/anime/");
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                //string to class object
                var listNameAnime = JsonSerializer.Deserialize<List<AnimeDTO>>(resultAnime, options);
                //step one check file
                foreach (var anime in listNameAnime)
                {
                    //get list episodes by name
                    var resultEpisode = await client.GetStringAsync($"{_protocol}://{_address}:{_port}/episode/name/{anime.Name}");

                    //string to object class
                    var listNameEpisode = JsonSerializer.Deserialize<List<EpisodeDTO>>(resultEpisode, options);

                    //foreach episodes
                    foreach (var episode in listNameEpisode)
                    {
                        string filePath = $"{_folder}/{episode.IDAnime}/Season {episode.NumberSeasonCurrent.ToString("D2")}/{episode.IDAnime} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString("D2")}.mp4";
                        logger.Debug($"check {filePath}");
                        
                        //check integry file
                        if (episode.StateDownload == null || episode.StateDownload == "failed" || (!File.Exists(filePath) && episode.StateDownload != "pending"))
                        {
                            //set pending to 
                            episode.StateDownload = "pending";
                            using (var content = new StringContent(JsonSerializer.Serialize(episode), System.Text.Encoding.UTF8, "application/json"))
                            {
                                client.PutAsync($"{_protocol}://{_address}:{_port}/statusDownload", content).GetAwaiter().GetResult();
                            }

                            //if file not exitst, send message to rabbit
                            await _publishEndpoint.Publish(episode);
                            logger.Info($"this file not exists, send message to DownloadService");
                        }
                    }
                }
                //step two check on website if the anime is still active
                foreach (var anime in listNameAnime)
                {
                    //get list episodes by name
                    var resultEpisode = await client.GetStringAsync($"{_protocol}://{_address}:{_port}/episode/name/{anime.Name}");

                    //string to object class
                    var listNameEpisode = JsonSerializer.Deserialize<List<EpisodeDTO>>(resultEpisode, options);
                    List<EpisodeDTO> checkEpisodes = null;

                    if (listNameEpisode.Count == 0)
                        checkEpisodes = HtmlAnimeSaturn.GetEpisodes(anime.UrlPage, anime.Name);
                    else
                        //check new episode
                        checkEpisodes = HtmlAnimeSaturn.GetEpisodes(listNameEpisode[listNameEpisode.Count - 1].Referer, anime.Name);

                    var listEpisodesAdd = new List<EpisodeDTO>(checkEpisodes);

                    foreach (var checkEpisode in checkEpisodes)
                    {
                        foreach (var episode in listNameEpisode)
                        {
                            if (episode.NumberEpisodeCurrent == checkEpisode.NumberEpisodeCurrent && episode.UrlVideo == checkEpisode.UrlVideo)
                            {
                                listEpisodesAdd.Remove(checkEpisode);
                                break;
                            }
                        }
                    }

                    if(listEpisodesAdd.Count > 0)
                    {
                        //insert to db
                        using (var content = new StringContent(JsonSerializer.Serialize(listEpisodesAdd), System.Text.Encoding.UTF8, "application/json"))
                        {
                            HttpResponseMessage result = client.PostAsync($"{_protocol}://{_address}:{_port}/episodes", content).Result;
                            if (result.StatusCode == HttpStatusCode.Created)
                                logger.Info($"Insert new item {await content.ReadAsStringAsync()}");
                            else if (result.StatusCode == HttpStatusCode.Conflict)
                                logger.Error($"Error insert new item for conflict");
                            else
                                logger.Error($"Error generic insert, message {await content.ReadAsStringAsync()}");
                        }

                        //send to rabbit
                        foreach (var episode in listEpisodesAdd)
                        {
                            await _publishEndpoint.Publish(episode);
                        }
                    }
                    //clear resource
                    listEpisodesAdd.Clear();
                }
                logger.Info("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(_timeRefresh, stoppingToken);
            }
        }
    }
}
