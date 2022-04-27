using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Application.Parallel;
using Cesxhin.AnimeSaturn.Domain.DTO;
using HtmlAgilityPack;
using NLog;
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
            byte[] imageBytes=null;

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
                imageBytes = webClient.DownloadData(imageUrl);
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
                Image = imageBytes,
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
    }
}
