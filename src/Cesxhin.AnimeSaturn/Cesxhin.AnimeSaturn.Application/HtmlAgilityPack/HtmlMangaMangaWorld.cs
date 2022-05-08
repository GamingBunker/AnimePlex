using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Application.Parallel;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using HtmlAgilityPack;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Cesxhin.AnimeSaturn.Application.HtmlAgilityPack
{
    public static class HtmlMangaMangaWorld
    {
        //log
        private static readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //parallel
        private static readonly ParallelManager<EpisodeDTO> parallel = new();

        public static HtmlDocument GetMangaHtml(string urlPage)
        {
            return new HtmlWeb().Load(urlPage);
        }

        public static MangaDTO GetManga(HtmlDocument doc, string urlPage)
        {
            string author=null, artist=null, type=null;
            bool status=false;
            int totalVolumes=0, totalChapters=0, releaseDate=0;
            string imageBase64 = null;

            _logger.Info($"Start download page manga: {urlPage}");

            var infoManga = doc.DocumentNode
                .SelectNodes("//section/div/div/div/div/div/div/div[2]/div[1]/div[@class='col-12 col-md-6']")
                .ToArray();

            //get info base of the anime
            string[] word = null;
            foreach(var info in infoManga)
            {
                word = info.InnerText.Split(new char[] { ':', '\n' });
                if (word[0].Contains("Autore"))
                {
                    author = word[1];

                }
                else if (word[0].Contains("Artista"))
                {
                    artist = word[1];
                }
                else if (word[0].Contains("Anno di uscita"))
                {
                    releaseDate = int.Parse(word[1]);
                }
                else if (word[0].Contains("Volumi totali"))
                {
                    totalVolumes = int.Parse(word[1]);
                }
                else if (word[0].Contains("Capitoli totali"))
                {
                    totalChapters = int.Parse(word[1]);
                }
                else if (word[0].Contains("Durata episodi"))
                {
                    totalChapters = int.Parse(word[1]);
                }
                else if (word[0].Contains("Tipo"))
                {
                    type = word[1];
                }else if (word[0].Contains("Stato"))
                {
                    if (word[1].Contains("Finito"))
                        status = true;
                    else
                        status = false;
                }
            }

            //get description
            var description = doc.DocumentNode
                .SelectNodes("//section/div/div/div/div/div/div[3]/div[2]")
                .First().InnerText;

            //get image
            var webClient = new WebClient();

            var imageUrl = doc.DocumentNode
                .SelectNodes("//section/div/div/div/div/div/div/div[1]/img")
                .First()
                .Attributes["src"].Value;
            try
            {
                var imageBytes = webClient.DownloadData(imageUrl);
                imageBase64 = Convert.ToBase64String(imageBytes);
            }
            catch
            {
                _logger.Error($"Error download image from this url {imageUrl}");
            }

            //get name manga
            var nameManga = doc.DocumentNode
                .SelectNodes("//section/div/div/div/div/div/div/div[2]/h1")
                .First().InnerText;

            return new MangaDTO
            {
                Artist = artist,
                Author = author,
                TotalChapters = totalChapters,
                Description = description,
                Image = imageBase64,
                Status = status,
                Type = type,
                UrlPage = urlPage,
                TotalVolumes = totalVolumes,
                DateRelease = releaseDate,
                Name = nameManga
            };
        }

        public static List<ChapterDTO> GetChapters(HtmlDocument doc, string urlPage, MangaDTO manga)
        {
            List<ChapterDTO> chaptersList = new();

            _logger.Info($"Start download chapters manga: {urlPage}");

            var volumes = doc.DocumentNode
                .SelectNodes("//section/div/div/div/div/div[2]/div/div[3]/div[@class='volume-element pl-2']")
                .ToArray();

            foreach (var volume in volumes)
            {
                var chapters = volume.SelectNodes("div[2]/div[@class='chapter']").ToArray();


                var numberCurrentVolume = volume
                    .SelectNodes("div")
                    .First().InnerText;
                var numberVolume = int.Parse(numberCurrentVolume.Split(new char[] { ' ', '\n' })[1]);

                foreach (var chapter in chapters)
                {
                    var link = chapter
                        .SelectNodes("a")
                        .First()
                        .Attributes["href"].Value;
                    var currentChapter = chapter
                        .SelectNodes("a/span")
                        .First()
                        .InnerText;

                    var numberCurrentChapter = float.Parse(currentChapter.Split(new char[] { ' ', '\n' })[1]);
                    chaptersList.Add(new ChapterDTO
                    {
                        UrlPage = link,
                        NameManga = manga.Name,
                        CurrentChapter = numberCurrentChapter,
                        CurrentVolume = numberVolume
                    });
                }
            }
            return chaptersList;
        }

        public static List<GenericUrl> GetMangaUrl(string name)
        {
            List<GenericUrl> listUrlManga = new();

            HtmlDocument doc;
            HtmlNode[] listManga;

            string url, imageUrl = null, urlPage = null, nameManga = null;

            var page = 1;
            while (true)
            {
                try
                {
                    url = $"https://www.mangaworld.in/archive?keyword={name}&page={page}";
                    doc = new HtmlWeb().Load(url);

                    listManga = doc.DocumentNode
                        .SelectNodes("//div/div/div/div[2]/div[@class='entry']")
                        .ToArray();

                    foreach(var manga in listManga)
                    {
                        //get image cover
                        imageUrl = manga
                            .SelectNodes("a/img")
                            .First()
                            .Attributes["src"].Value;

                        //url page
                        urlPage = manga
                            .SelectNodes("a")
                            .First()
                            .Attributes["href"].Value;

                        //name
                        nameManga = manga
                            .SelectNodes("div/p")
                            .First().InnerText;

                        listUrlManga.Add(new GenericUrl
                        {
                            Name = nameManga,
                            Url = urlPage,
                            UrlImage = imageUrl,
                            TypeView = "manga"
                        });
                    }
                }
                catch
                {
                    //not found other pages
                    return listUrlManga;
                }

                page++;
            }
        }
    }
}
