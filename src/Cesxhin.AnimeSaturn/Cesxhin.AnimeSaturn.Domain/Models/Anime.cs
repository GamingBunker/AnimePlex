using Cesxhin.AnimeSaturn.Domain.DTO;
using RepoDb.Attributes;
using System;

namespace Cesxhin.AnimeSaturn.Domain.Models
{
    [Map("anime")]
    public class Anime
    {
        [Identity]
        [Map("author")]
        public string Author { get; set; }
        [Map("name")]
        public string Name { get; set; }
        [Map("description")]
        public string Description { get; set; }
        [Map("vote")]
        public string Vote { get; set; }
        [Map("status")]
        public bool Finish { get; set; }
        [Map("durationepisode")]
        public string DurationEpisode { get; set; }
        [Map("episodetotal")]
        public int EpisodeTotal { get; set; }
        [Map("daterelease")]
        public DateTime DateRelease { get; set; }
        [Map("image")]
        public byte[] Image { get; set; }
        [Map("urlpage")]
        public string UrlPage { get; set; }

        //convert AnimeDTO to Anime
        public Anime AnimeDTOToAnime(AnimeDTO anime)
        {
            return new Anime
            {
                Name = anime.Name,
                Description = anime.Description,
                Vote = anime.Vote,
                DurationEpisode = anime.DurationEpisode,
                EpisodeTotal = anime.EpisodeTotal,
                DateRelease = anime.DateRelease,
                Image = anime.Image,
                Finish = anime.Finish,
                UrlPage = anime.UrlPage,
                Author = anime.Author
            };
        }
    }
}
