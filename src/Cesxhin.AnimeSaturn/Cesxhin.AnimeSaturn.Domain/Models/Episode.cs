using Cesxhin.AnimeSaturn.Domain.DTO;
using RepoDb.Attributes;

namespace Cesxhin.AnimeSaturn.Domain.Models
{
    [Map("episode")]
    public class Episode
    {
        [Primary]
        [Map("id")]
        public int ID { get; set; }
        [Map("idanime")]
        public string IDAnime { get; set; }
        [Map("urlvideo")]
        public string UrlVideo { get; set; }
        [Map("referer")]
        public string Referer { get; set; }
        [Map("numberepisodecurrent")]
        public int NumberEpisodeCurrent { get; set; }

        //convert EpisodeDTO to Episode
        public Episode EpisodeDTOToEpisode(EpisodeDTO episode)
        {
            return new Episode
            {
                ID = episode.ID,
                IDAnime = episode.IDAnime,
                UrlVideo = episode.UrlVideo,
                NumberEpisodeCurrent = episode.NumberEpisodeCurrent,
                Referer = episode.Referer
                
            };
        }
    }
}
