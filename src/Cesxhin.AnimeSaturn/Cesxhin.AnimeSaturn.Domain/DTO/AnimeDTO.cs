using Cesxhin.AnimeSaturn.Domain.Models;
using System;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class AnimeDTO
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Studio { get; set; }
        public string Description { get; set; }
        public string Vote { get; set; }
        public bool Finish { get; set; }
        public string DurationEpisode { get; set; }
        public int EpisodeTotal { get; set; }
        public DateTime DateRelease { get; set; }
        public string Image { get; set; }
        public string UrlPage { get; set; }
        public string TypeView { get; } = "anime";

        //convert Anime to AnimeDTO
        public static AnimeDTO AnimeToAnimeDTO(Anime anime)
        {
            return new AnimeDTO
            {
                Name = anime.Name,
                Description = anime.Description,
                Vote = anime.Vote,
                DurationEpisode = anime.DurationEpisode,
                EpisodeTotal = anime.EpisodeTotal,
                DateRelease = anime.DateRelease,
                Finish = anime.Finish,
                Image = anime.Image,
                UrlPage = anime.UrlPage,
                Studio = anime.Studio,
                Surname = anime.Surname
            };
        }

    }
}
