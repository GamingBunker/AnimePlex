using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Application.Parallel;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using HtmlAgilityPack;
using m3uParser;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Cesxhin.AnimeSaturn.Application.HtmlAgilityPack
{
    public static class HtmlAnimeSaturn
    {
        //log
        private static readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //parallel
        private static readonly ParallelManager<EpisodeDTO> parallel = new();

        public static AnimeDTO GetAnime(string urlPage)
        {
            //set variable
            string durationEpisode = null, vote = null, description = null, surname = null, nameAnime = null, numberTotalEpisodes = null, date = null, studio = null;
            bool finish = false;
            string imageBase64=null;

            _logger.Info($"Start download page anime: {urlPage}");

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
                if (words[i].Replace(" ","").ToLower() == "studio")
                {
                    studio = words[i + 1];

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
                .SelectNodes("//div/div/div[2]/div/div[@class='box-trasparente-alternativo rounded']")
                .First()
                .InnerText;

            // get surname
             surname = doc.DocumentNode
                 .SelectNodes("//div/div/div[2]/div/b")
                 .First()
                 .InnerText;

            if(surname.Contains("Sub ITA"))
            {
                surname = surname.Replace(" Sub ITA", "");
            }else if(surname.Contains(")  ITA"))
            {
                surname = surname.Replace(")  ITA", "");
                surname = surname.Replace("(ITA", "(ITA)");
            }

            //get image
            var webClient = new WebClient();

            var imageUrl = doc.DocumentNode
                .SelectNodes("//div/div/div/div[2]/img")
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

            var resultBool = int.TryParse(numberTotalEpisodes, out int resultEpisodeTotal);
            if (!resultBool)
            {
                resultEpisodeTotal = 1;
            }

            _logger.Info($"End download page anime: {urlPage}");

            return new AnimeDTO
            {
                Surname = surname,
                DateRelease = DateTime.Parse(date),
                DurationEpisode = durationEpisode,
                Vote = vote,
                EpisodeTotal = resultEpisodeTotal,
                Finish = finish,
                Description = description,
                Name = RemoveSpecialCharacters(nameAnime),
                Image = imageBase64,
                Studio = studio,
                UrlPage = urlPage
            };
        }

        public static List<EpisodeDTO> GetEpisodes(string urlPage, string name)
        {
            //set variable
            List<EpisodeDTO> episodes = new();
            int numberSeason = 1; //default

            _logger.Info($"Start download page episode: {urlPage}");

            //get page
            HtmlDocument doc = new HtmlWeb().Load(urlPage);

            /*string numberSeasonString = Regex.Match(name, @"\d+").Value; <-- problem set number season
            if (numberSeasonString.Length > 0)
                numberSeason = int.Parse(numberSeasonString);*/

            int rangeAnime = 0;
            try
            {
                List<HtmlNode> listEpisodes;
                int currentNumberEpisodes = 0;

                List<Func<EpisodeDTO>> tasks = new();
                do
                {
                    //check group episodes
                    try
                    {
                        listEpisodes = doc.DocumentNode
                       .SelectNodes("//div/div/div[2]/div[5]/div/div/div[@id='range-anime-" + rangeAnime + "']/div")
                       .ToList();
                    }
                    catch
                    {
                        parallel.AddTasks(tasks);
                        parallel.Start();
                        parallel.WhenCompleted();
                        var results = parallel.GetResultAndClear();

                        foreach (var result in results)
                            episodes.Add(result);

                        break;
                    }

                    for (int i = 0; i < listEpisodes.Count; i++)
                    {
                        var numberEpisode = currentNumberEpisodes + i + 1;
                        var episodeTask = listEpisodes[i];
                        tasks.Add(new Func<EpisodeDTO>(() => { return DownloadMetadataEpisodeAsync(episodeTask, numberSeason, numberEpisode, urlPage, name);  }));
                    }
                    currentNumberEpisodes += listEpisodes.Count;
                    rangeAnime++;
                } while (true);
            }catch(ArgumentNullException e)
            {
                _logger.Error($"Argument Null, details: {e.Message}");
                return null;
            }catch(Exception e)
            {
                _logger.Fatal($"Error generic: {e.Message}");
                return null;
            }

            _logger.Info($"End download page episode: {urlPage}");

            return episodes;
        }

        private static EpisodeDTO DownloadMetadataEpisodeAsync(HtmlNode episode, int numberSeason, int numberEpisode, string urlPage, string name)
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

            string url = null;
            PlayerUrl playerUrl = null;

            try
            {
                _logger.Debug($"Try download url with file: {urlPage}");

                HtmlDocument docVideo = new HtmlWeb().Load(urlVideo);

                url = docVideo.DocumentNode
                    .SelectNodes("//center/div[2]/div/div/div/div/video/source")
                    .First()
                    .Attributes["src"].Value;
                _logger.Debug($"Done download url with file: {urlPage}");
            }
            catch (ArgumentNullException)
            {
                _logger.Debug($"Failed download url with file: {urlPage}");
                _logger.Debug($"Try download url with buffer: {urlPage}");

                HtmlDocument docVideo = new HtmlWeb().Load(urlVideo);

                string urlLocal = docVideo.DocumentNode
                    .SelectNodes("//center/div[2]/div/div/div/div/div/div/script[2]")
                    .First().InnerText;

                //get url list source
                urlLocal = urlLocal.Replace("jwplayer('player_hls').setup(", " ");

                urlLocal = urlLocal.Replace(");", " ");

                urlLocal = urlLocal.Replace(".replace(\"playlist.m3u8\", \"thumbnails.vtt\")", " ");

                urlLocal = urlLocal.Replace(".replace(\"playlist.m3u8\", \"poster.jpg\")", " ");
                urlLocal = urlLocal.Replace("'", "\"");
                urlLocal = urlLocal.Replace("file", "\"Playlist\"");
                urlLocal = urlLocal.Replace("tracks", "\"tracks\"");
                urlLocal = urlLocal.Replace("kind", "\"kind\"");
                urlLocal = urlLocal.Replace("image", "\"image\"");
                urlLocal = urlLocal.Replace("preload", "\"preload\"");
                urlLocal = urlLocal.Replace("abouttext", "\"abouttext\"");
                urlLocal = urlLocal.Replace("aboutlink", "\"aboutlink\"");
                urlLocal = urlLocal.Replace("playbackRateControls", "\"playbackRateControls\"");
                urlLocal = urlLocal.Replace("sharing", "\"sharing\"");
                urlLocal = urlLocal.Replace("heading", "\"heading\"");

                playerUrl = JsonSerializer.Deserialize<PlayerUrl>(urlLocal);

                playerUrl.BaseUrl = playerUrl.Playlist.Replace("/playlist.m3u8", "");

                //download source files
                WebClient client = new();
                var bytes = client.DownloadData(playerUrl.Playlist);
                var sourceFiles = System.Text.Encoding.UTF8.GetString(bytes);

                var contentM3u = M3U.Parse(sourceFiles);
                string file = contentM3u.Warnings.First();

                playerUrl.PlaylistSources = file.Substring(file.LastIndexOf("./") + 1);
                playerUrl.Resolution = playerUrl.PlaylistSources.Substring(1, playerUrl.PlaylistSources.IndexOf("p"));

                //get list bytes for file
                bytes = client.DownloadData(playerUrl.BaseUrl + playerUrl.PlaylistSources);
                sourceFiles = System.Text.Encoding.UTF8.GetString(bytes);
                contentM3u = M3U.Parse(sourceFiles);
                playerUrl.endNumberBuffer = contentM3u.Medias.Count() - 1; //start 0 to xx

                _logger.Debug($"Done download url with buffer: {urlPage}");
            }

            if (playerUrl != null)
            {
                return new EpisodeDTO
                {
                    ID = $"{name}-s{numberSeason}-e{numberEpisode}",
                    AnimeId = name,
                    NumberEpisodeCurrent = numberEpisode,
                    BaseUrl = playerUrl.BaseUrl,
                    Playlist = playerUrl.Playlist,
                    PlaylistSources = playerUrl.PlaylistSources,
                    Resolution = playerUrl.Resolution,
                    NumberSeasonCurrent = numberSeason,
                    endNumberBuffer = playerUrl.endNumberBuffer
                };
            }
            else
            {
                return new EpisodeDTO
                {
                    ID = $"{name}-s{numberSeason}-e{numberEpisode}",
                    AnimeId = name,
                    UrlVideo = url,
                    NumberEpisodeCurrent = numberEpisode,
                    NumberSeasonCurrent = numberSeason

                };
            }
        }

        //get list anime external
        public static List<GenericUrl> GetAnimeUrl(string name)
        {
            _logger.Info($"Start download lsit anime, search: {name}");
            //get page
            HtmlDocument doc = new HtmlWeb().Load("https://www.animesaturn.it/animelist?search=" + name);

            //get number find elements
            string results = doc.DocumentNode
                .SelectNodes("//div/div/span/span/b[2]")
                .First().InnerText;

            //int numberAnime = int.Parse(results);

            //get animes
            var animes = doc.DocumentNode
                .SelectNodes("//ul/li/div")
                .ToList();

            List<GenericUrl> animeUrl = new();
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

                    animeUrl.Add(new GenericUrl
                    {
                        Name = RemoveSpecialCharacters(nameAnime),
                        Url = linkPage,
                        UrlImage = linkCopertina,
                        TypeView = "anime"
                    });

                }
                catch { /*ignore other link a */ }
            }

            _logger.Info($"End download lsit anime, search: {name}");
            return animeUrl;
        }

        public static string RemoveSpecialCharacters(string str)
        {
            //remove character special
            return Regex.Replace(str, "[^a-zA-Z0-9_() ]+", "", RegexOptions.Compiled);
        }
    }
}
