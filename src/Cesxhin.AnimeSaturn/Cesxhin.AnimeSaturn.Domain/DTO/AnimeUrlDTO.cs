using Cesxhin.AnimeSaturn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
