using Cesxhin.AnimeSaturn.Domain.Models;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class EpisodeDTO
    {
        public int ID { get; set; }
        public string IDAnime { get; set; }
        public string UrlVideo { get; set; }
        public string Referer { get; set; }
        public int NumberEpisodeCurrent { get; set; }

        public EpisodeDTO EpisodeToEpisodeDTO(Episode episode)
        {
            return new EpisodeDTO
            {
                ID = episode.ID,
                IDAnime = episode.IDAnime,
                UrlVideo = episode.UrlVideo,
                NumberEpisodeCurrent = episode.NumberEpisodeCurrent,
                Referer = episode.Referer,
            };
        }
    }
}
