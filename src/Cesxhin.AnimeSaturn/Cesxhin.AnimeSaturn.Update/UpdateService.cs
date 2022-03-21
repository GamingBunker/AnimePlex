using Cesxhin.AnimeSaturn.Domain.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Update
{
    internal class UpdateService
    {
        const string _folder = "G:\\TestAnime";
        public UpdateService()
        {
            HttpClient client = new HttpClient();
            while (true)
            {
                //download api
                var resultAnime = client.GetStringAsync("https://localhost:44300/anime/").GetAwaiter().GetResult();
                var resultEpisode = client.GetStringAsync("https://localhost:44300/episodes/").GetAwaiter().GetResult();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };

                //string to class object
                var listNameAnime = JsonSerializer.Deserialize<List<AnimeDTO>>(resultAnime, options);
                var listNameEpisode = JsonSerializer.Deserialize<List<EpisodeDTO>>(resultEpisode, options);

                foreach (var episode in listNameEpisode)
                {
                    foreach (var anime in listNameAnime)
                    {
                        if (episode.IDAnime == anime.Name)
                        {
                            string[] path = episode.UrlVideo.Split('/');
                            if (!File.Exists($"{_folder}\\{anime.Name}\\{path[path.Length]}"))
                            {

                            }
                        }
                    }
                }

                Thread.Sleep(30000);
            }
        }
    }
}
