using Cesxhin.AnimeSaturn.Domain.Models;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public  class GenericUrlDTO
    {
        public string Name { get; set; }
        public string UrlPageDownload { get; set; }
        public string Image { get; set; }
        public string TypeView { get; set; }
        public bool Exists { get; set; } = false;

        //convert GenericUrl to GenericUrlDTO
        public static GenericUrlDTO GenericUrlToGenericUrlDTO(GenericUrl anime)
        {
            return new GenericUrlDTO
            {
                Name = anime.Name,
                UrlPageDownload = anime.Url,
                Image = anime.UrlImage,
                TypeView = anime.TypeView
            };
        }
    }
}
