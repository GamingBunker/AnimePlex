using Cesxhin.AnimeSaturn.Domain.Models;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public  class AnimeUrlDTO
    {
        public string Name { get; set; }
        public string UrlPageDownload { get; set; }
        public string Image { get; set; }

        public AnimeUrlDTO AnimeToAnimeUrlDTO(AnimeUrl anime)
        {
            return new AnimeUrlDTO
            {
                Name = anime.Name,
                UrlPageDownload = anime.Url,
                Image = anime.UrlImage
            };
        }
    }
}
