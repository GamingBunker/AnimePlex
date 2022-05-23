using Cesxhin.AnimeSaturn.Domain.Models;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class MangaDTO
    {
        public string Name { get; set; }
        public string Anime { get; set; }
        public string Author { get; set; }
        public string Artist { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public int TotalVolumes { get; set; }
        public int TotalChapters { get; set; }
        public int DateRelease { get; set; }
        public string Image { get; set; }
        public string UrlPage { get; set; }

        //convert Manga to MangaDTO
        public static MangaDTO MangaToMangaDTO(Manga manga)
        {
            return new MangaDTO
            {
                Name = manga.Name,
                Anime = manga.Anime,
                Author = manga.Author,
                Artist = manga.Artist,
                Type = manga.Type,
                Description = manga.Description,
                TotalVolumes = manga.TotalVolumes,
                TotalChapters = manga.TotalChapters,
                DateRelease = manga.DateRelease,
                Image = manga.Image,
                UrlPage = manga.UrlPage
            };
        }
    }
}
