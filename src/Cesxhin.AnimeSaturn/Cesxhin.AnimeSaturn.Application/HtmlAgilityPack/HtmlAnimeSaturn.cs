using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using HtmlAgilityPack;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Cesxhin.AnimeSaturn.Application.HtmlAgilityPack
{
    public static class HtmlAnimeSaturn
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static AnimeDTO GetAnime(string urlPage)
        {
            //set variable
            string durationEpisode = null, vote = null, description = null, nameAnime = null, numberTotalEpisodes = null, date = null, author = null;
            bool finish = false;
            byte[] imageBytes = null;

            //get page
            HtmlDocument doc = new HtmlWeb().Load(urlPage);

            //get info general
            var infoAnime = doc.DocumentNode
                .SelectNodes("//div/div/div[@class='container shadow rounded bg-dark-as-box mb-3 p-3 w-100 text-white']")
                .First().InnerText;

            //get info base of the anime
            string[] words = infoAnime.Split(new char[] { ':', '\n' });
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Contains("Autore"))
                {
                    author = words[i + 1];

                }
                else if (words[i].Contains("Stato"))
                {
                    if (words[i + 1].Contains("Finito"))
                        finish = true;

                }
                else if (words[i].Contains("Data di uscita"))
                {
                    if (words[i + 1].Contains("Gennaio"))
                        words[i + 1] = words[i + 1].Replace("Gennaio", "Jan");
                    else if (words[i + 1].Contains("Febbraio"))
                        words[i + 1] = words[i + 1].Replace("Febbraio", "Feb");
                    else if (words[i + 1].Contains("Marzo"))
                        words[i + 1] = words[i + 1].Replace("Marzo", "Mar");
                    else if (words[i + 1].Contains("Aprile"))
                        words[i + 1] = words[i + 1].Replace("Aprile", "Apr");
                    else if (words[i + 1].Contains("Maggio"))
                        words[i + 1] = words[i + 1].Replace("Maggio", "May");
                    else if (words[i + 1].Contains("Giugno"))
                        words[i + 1] = words[i + 1].Replace("Giugno", "Jun");
                    else if (words[i + 1].Contains("Luglio"))
                        words[i + 1] = words[i + 1].Replace("Luglio", "Jul");
                    else if (words[i + 1].Contains("Agosto"))
                        words[i + 1] = words[i + 1].Replace("Agosto", "Aug");
                    else if (words[i + 1].Contains("Settembre"))
                        words[i + 1] = words[i + 1].Replace("Settembre", "Sep");
                    else if (words[i + 1].Contains("Ottobre"))
                        words[i + 1] = words[i + 1].Replace("Ottobre", "Oct");
                    else if (words[i + 1].Contains("Novembre"))
                        words[i + 1] = words[i + 1].Replace("Novembre", "Nov");
                    else if (words[i + 1].Contains("Dicembre"))
                        words[i + 1] = words[i + 1].Replace("Dicembre", "Dec");

                    date = words[i + 1];
                }
                else if (words[i].Contains("Episodi"))
                {
                    numberTotalEpisodes = words[i + 1];
                }
                else if (words[i].Contains("Durata episodi"))
                {
                    durationEpisode = words[i + 1];
                }
                else if (words[i].Contains("Voto"))
                {
                    vote = words[i + 1];
                }
            }

            //get description
            try
            {
                description = doc.DocumentNode
                    .SelectNodes("//div/div/div[2]/div[4]/div/div[@id='full-trama']")
                    .First().InnerText;
            }
            catch
            {
                description = doc.DocumentNode
                       .SelectNodes("//div/div/div[2]/div[4]/div/div[@id='shown-trama']")
                       .First().InnerText;
            }

            //get name
            nameAnime = doc.DocumentNode
                .SelectNodes("//div/div/div[2]/div/b")
                .First().InnerText;

            //get image
            var webClient = new WebClient();

            var imageUrl = doc.DocumentNode
                .SelectNodes("//div/div/div/div[2]/img")
                .First()
                .Attributes["src"].Value;
            try
            {
                imageBytes = webClient.DownloadData(imageUrl);
            }
            catch
            {
                logger.Error($"Error download image from this url {imageUrl}");
            }

            var resultBool = int.TryParse(numberTotalEpisodes, out int resultEpisodeTotal);
            if (!resultBool)
            {
                resultEpisodeTotal = 1;
            }

            return new AnimeDTO
            {
                DateRelease = DateTime.Parse(date),
                DurationEpisode = durationEpisode,
                Vote = vote,
                EpisodeTotal = resultEpisodeTotal,
                Finish = finish,
                Description = description,
                Name = RemoveSpecialCharacters(nameAnime),
                Image = imageBytes,
                Author = author,
                UrlPage = urlPage
            };
        }

        public static List<EpisodeDTO> GetEpisodes(string urlPage, string name)
        {
            //get page
            HtmlDocument doc = new HtmlWeb().Load(urlPage);

            List<EpisodeDTO> episodes = new List<EpisodeDTO>();
            var listEpisodes = doc.DocumentNode
                .SelectNodes("//div/div/div[2]/div[5]/div/div/div/div")
                .ToList();

            int numberEpisode = 1;
            foreach (var episode in listEpisodes)
            {
                string urlEpisode = episode.
                    SelectNodes("a")
                    .First()
                    .Attributes["href"].Value;

                HtmlDocument docEpisode = new HtmlWeb().Load(urlEpisode);

                string urlVideo = docEpisode.DocumentNode
                    .SelectNodes("//div[@class='container p-3 shadow rounded bg-dark-as-box']/div/div/a[1]")
                    .First()
                    .Attributes["href"].Value;

                HtmlDocument docVideo = new HtmlWeb().Load(urlVideo);
                string url = docVideo.DocumentNode
                    .SelectNodes("//center/div[2]/div/div/div/div/video/source")
                    .First()
                    .Attributes["src"].Value;

                episodes.Add(new EpisodeDTO
                {
                    IDAnime = name,
                    UrlVideo = url,
                    Referer = urlPage,
                    NumberEpisodeCurrent = numberEpisode

                });
                numberEpisode++;
            }
            return episodes;
        }

        public static List<AnimeUrl> GetAnimeUrl(string name)
        {
            //get page
            HtmlDocument doc = new HtmlWeb().Load("https://www.animesaturn.it/animelist?search=" + name);

            //get number find elements
            string results = doc.DocumentNode
                .SelectNodes("//div/div/span/span/b[2]")
                .First().InnerText;

            int numberAnime = int.Parse(results);

            //get animes
            var animes = doc.DocumentNode
                .SelectNodes("//ul/li/div")
                .ToList();

            var animeUrl = new List<AnimeUrl>();
            foreach (var anime in animes)
            {
                try
                {
                    //get link page
                    var linkPage = anime
                        .SelectNodes("a")
                        .First()
                        .Attributes["href"].Value;

                    //get anime
                    var nameAnime = anime
                        .SelectNodes("div/h3/a")
                        .First().InnerText;

                    //get url
                    var linkCopertina = anime
                        .SelectNodes("a/img[2]")
                        .First()
                        .Attributes["src"].Value;

                    animeUrl.Add(new AnimeUrl
                    {
                        Name = nameAnime,
                        Url = linkPage,
                        UrlImage = linkCopertina
                    });

                }
                catch { /*ignore other link a */ }
            }
            return animeUrl;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            //remove character special
            return Regex.Replace(str, "[^a-zA-Z0-9_.() ]+", "", RegexOptions.Compiled);
        }
    }
}
