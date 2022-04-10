using Cesxhin.AnimeSaturn.Domain.Models;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class EpisodeRegisterDTO
    {
        public string EpisodeId { get; set; }
        public string EpisodePath { get; set; }
        public string EpisodeHash { get; set; }

        public static EpisodeRegisterDTO EpisodeRegisterToEpisodeRegisterDTO(EpisodeRegister anime)
        {
            return new EpisodeRegisterDTO
            {
                EpisodeId = anime.EpisodeId,
                EpisodePath = anime.EpisodePath,
                EpisodeHash = anime.EpisodeHash
            };
        }
    }
}
