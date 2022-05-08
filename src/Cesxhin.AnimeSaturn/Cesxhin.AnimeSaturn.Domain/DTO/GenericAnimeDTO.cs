using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class GenericAnimeDTO
    {
        //Anime
        public AnimeDTO Anime { get; set; }
        public List<EpisodeDTO> Episodes { get; set; }
        public List<EpisodeRegisterDTO> EpisodeRegister { get; set; }
    }
}
