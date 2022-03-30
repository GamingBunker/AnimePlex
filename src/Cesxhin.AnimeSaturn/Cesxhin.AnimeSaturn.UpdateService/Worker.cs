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
            HttpResponseMessage resultHttp;

            //set number view
            string formatNumberView = "D2";

            while (!stoppingToken.IsCancellationRequested)
            {
                List<AnimeDTO> listNameAnime = new List<AnimeDTO>();

                //settings deserialize
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                //download api
                resultHttp = await client.GetAsync($"{_protocol}://{_address}:{_port}/anime");
                if (resultHttp.IsSuccessStatusCode)
                {

                    //string to class object
                    listNameAnime = JsonSerializer.Deserialize<List<AnimeDTO>>(await resultHttp.Content.ReadAsStringAsync(), options);

                }
                else if (resultHttp.StatusCode == HttpStatusCode.NotFound)
                {
                    logger.Warn("Not found any anime");

                } else
                {
                    logger.Error("Can't get api anime, details: " + resultHttp.StatusCode);
                }

                //step one check file
                foreach (var anime in listNameAnime)
                {
                    //get list episodes by name
                    List<EpisodeDTO> listNameEpisode = new List<EpisodeDTO>();

                    resultHttp = await client.GetAsync($"{_protocol}://{_address}:{_port}/episode/name/{anime.Name}");
                    if (resultHttp.IsSuccessStatusCode)
                    {
                        //string to object class
                        listNameEpisode = JsonSerializer.Deserialize<List<EpisodeDTO>>(await resultHttp.Content.ReadAsStringAsync(), options);
                    }
                    else if (resultHttp.StatusCode == HttpStatusCode.NotFound)
                    {
                        continue;
                    }

                    //foreach episodes
                    foreach (var episode in listNameEpisode)
                    {
                        if (episode.NumberEpisodeCurrent > 99)
                            formatNumberView = "D3";
                        else if (episode.NumberEpisodeCurrent > 999)
                            formatNumberView = "D4";

                        string filePath = $"{_folder}/{episode.IDAnime}/Season {episode.NumberSeasonCurrent.ToString("D2")}/{episode.IDAnime} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString(formatNumberView)}.mp4";
                        logger.Debug($"check {filePath}");

                        //check integry file
                        if (episode.StateDownload == null || episode.StateDownload == "failed" || (!File.Exists(filePath) && episode.StateDownload != "pending"))
                        {
                            if (await ConfirmStartDownloadAnime(episode))
                            {
                                logger.Info($"ok {episode.IDAnime} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString("D2")}");
                            } else
                            {
                                logger.Info($"Error publish {episode.IDAnime} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString("D2")}");
                            }
                        }
                    }
                }

                //step two check on website if the anime is still active
                foreach (var anime in listNameAnime)
                {
                    //get list episodes by name
                    List<EpisodeDTO> listNameEpisode = new List<EpisodeDTO>();
                    List<EpisodeDTO> checkEpisodes = null;
                    List<EpisodeDTO> listEpisodesAdd = null;

                    //get current number episodes
                    resultHttp = await client.GetAsync($"{_protocol}://{_address}:{_port}/episode/name/{anime.Name}");
                    if (resultHttp.IsSuccessStatusCode)
                    {
                        listNameEpisode = JsonSerializer.Deserialize<List<EpisodeDTO>>(await resultHttp.Content.ReadAsStringAsync(), options);
                    }

                    if (listNameEpisode.Count == 0)
                    {
                        logger.Warn($"Anime of {anime.Name} not have any episodes, try re-download all episodes");

                        //get all episodes
                        checkEpisodes = HtmlAnimeSaturn.GetEpisodes(anime.UrlPage, anime.Name);

                        //check if null
                        if (checkEpisodes == null)
                        {
                            logger.Error($"Can't download with this url, {anime.UrlPage}");
                            continue;
                        }
                    }
                    else
                    {
                        logger.Info("Check new episodes for Anime: " + anime.Name);

                        //check new episode
                        checkEpisodes = HtmlAnimeSaturn.GetEpisodes(listNameEpisode[listNameEpisode.Count - 1].Referer, anime.Name);

                        //check if null
                        if (checkEpisodes == null)
                        {
                            logger.Error($"Can't download with this url, {listNameEpisode[listNameEpisode.Count - 1].Referer}");
                            continue;
                        }
                    }

                    listEpisodesAdd = new List<EpisodeDTO>(checkEpisodes);

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

                    if (listEpisodesAdd.Count > 0)
                    {
                        //insert to db
                        using (var content = new StringContent(JsonSerializer.Serialize(listEpisodesAdd), System.Text.Encoding.UTF8, "application/json"))
                        {
                            resultHttp = await client.PostAsync($"{_protocol}://{_address}:{_port}/episodes", content);
                            if (resultHttp.StatusCode == HttpStatusCode.Created)
                            {
                                logger.Info($"Insert new item {await content.ReadAsStringAsync()}");
                                listEpisodesAdd = JsonSerializer.Deserialize<List<EpisodeDTO>>(await resultHttp.Content.ReadAsStringAsync(), options);
                            }
                            else if (resultHttp.StatusCode == HttpStatusCode.Conflict)
                                logger.Error($"Error insert new item becouse there is one conflict, {resultHttp.Content.ReadAsStringAsync()}");
                            else
                                logger.Error($"Error generic insert, payload: {resultHttp.Content.ReadAsStringAsync()}");
                        }

                        //send to rabbit
                        foreach (var episode in listEpisodesAdd)
                        {
                            formatNumberView = "D2";
                            if (episode.NumberEpisodeCurrent > 99)
                                formatNumberView = "D3";
                            else if (episode.NumberEpisodeCurrent > 999)
                                formatNumberView = "D4";

                            if (await ConfirmStartDownloadAnime(episode))
                            {
                                logger.Info($"ok {episode.IDAnime} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString(formatNumberView)}");
                            }
                            else
                            {
                                logger.Info($"Error publish {episode.IDAnime} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString(formatNumberView)}");
                            }
                        }
                    }
                    //clear resource
                    listEpisodesAdd.Clear();
                }
                logger.Info("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(_timeRefresh, stoppingToken);
            }
        }

        private async Task<bool> ConfirmStartDownloadAnime(EpisodeDTO episode)
        {
            //set pending to 
            episode.StateDownload = "pending";

            using(var client = new HttpClient())
            using (var content = new StringContent(JsonSerializer.Serialize(episode), System.Text.Encoding.UTF8, "application/json"))
            {
                var resultHttp = await client.PutAsync($"{_protocol}://{_address}:{_port}/statusDownload", content);
                if (resultHttp.IsSuccessStatusCode)
                {
                    //if file not exitst, send message to rabbit
                    await _publishEndpoint.Publish(episode);
                    logger.Info($"this file does not exists, sending message to DownloadService");
                    return true;
                }
                else
                {
                    logger.Info($"Not can change state download of someone episode, details" + JsonSerializer.Serialize(episode));
                }
            }
            return false;
        }
    }
}
