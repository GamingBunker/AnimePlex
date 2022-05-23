using Cesxhin.AnimeSaturn.Domain.DTO;
using RepoDb.Attributes;
using System;

namespace Cesxhin.AnimeSaturn.Domain.Models
{
    [Map("manga")]
    public class Manga
    {
        [Identity]
        [Map("name")]
        public string Name { get; set; }

        [Map("anime")]
        public string Anime { get; set; }

        [Map("author")]
        public string Author { get; set; }

        [Map("artist")]
        public string Artist { get; set; }

        [Map("type")]
        public string Type { get; set; }

        [Map("description")]
        public string Description { get; set; }

        [Map("status")]
        public bool Status { get; set; }

        [Map("totalvolumes")]
        public int TotalVolumes { get; set; }

        [Map("totalchapters")]
        public int TotalChapters { get; set; }

        [Map("daterelease")]
        public int DateRelease { get; set; }

        [Map("image")]
        public string Image { get; set; }

        [Map("urlpage")]
        public string UrlPage { get; set; }

        //convert MangaDTO to Manga
        public static Manga MangaDTOToManga(MangaDTO manga)
        {
            return new Manga
            {
                Name = manga.Name,
                Anime = manga.Anime,
                Author = manga.Author,
                Artist = manga.Artist,
                Type = manga.Type,
                TotalChapters = manga.TotalChapters,
                DateRelease = manga.DateRelease,
                Description = manga.Description,
                Status = manga.Status,
                Image = manga.Image,
                UrlPage = manga.UrlPage,
                TotalVolumes = manga.TotalVolumes
            };
        }
    }
}
