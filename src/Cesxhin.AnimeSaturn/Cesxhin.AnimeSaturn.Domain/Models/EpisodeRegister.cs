using Cesxhin.AnimeSaturn.Domain.DTO;
using RepoDb.Attributes;

namespace Cesxhin.AnimeSaturn.Domain.Models
{
    [Map("episoderegister")]
    public class EpisodeRegister
    {
        [Primary]
        [Map("id")]
        public int ID { get; set; }

        [Map("episodeid")]
        public int EpisodeId { get; set; }

        [Map("episodepath")]
        public string EpisodePath { get; set; }

        [Map("episodehash")]
        public string EpisodeHash { get; set; }

        public static EpisodeRegister EpisodeRegisterToEpisodeRegisterDTO(EpisodeRegisterDTO anime)
        {
            return new EpisodeRegister
            {
                ID = anime.ID,
                EpisodeId = anime.EpisodeId,
                EpisodePath = anime.EpisodePath,
                EpisodeHash = anime.EpisodeHash
            };
        }
    }
}
