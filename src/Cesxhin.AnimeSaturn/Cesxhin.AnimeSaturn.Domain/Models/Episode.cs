using Cesxhin.AnimeSaturn.Domain.DTO;
using RepoDb.Attributes;

namespace Cesxhin.AnimeSaturn.Domain.Models
{
    [Map("episode")]
    public class Episode
    {
        [Primary]
        [Map("id")]
        public string ID { get; set; }

        [Map("animeid")]
        public string AnimeId { get; set; }

        [Map("urlvideo")]
        public string UrlVideo { get; set; }

        [Map("numberepisodecurrent")]
        public int NumberEpisodeCurrent { get; set; }

        [Map("numberseasoncurrent")]
        public int NumberSeasonCurrent { get; set; }

        [Map("statedownload")]
        public string StateDownload { get; set; }

        [Map("percentualdownload")]
        public int PercentualDownload { get; set; }

        /*
         Download playlist: https://www.saturnspeed49.org/DDL/ANIME/HatarakuSaibou2/01/playlist.m3u8
         Select Resolution for download source: https://www.saturnspeed49.org/DDL/ANIME/HatarakuSaibou2/01/360p/playlist_360p.m3u8
         Download source: https://www.saturnspeed49.org/DDL/ANIME/HatarakuSaibou2/01/360p/360p-000.ts
         */

        //alternative source
        [Map("baseurl")]
        public string BaseUrl { get; set; }

        [Map("playlist")]
        public string Playlist { get; set; }

        [Map("resolution")]
        public string Resolution { get; set; }

        [Map("playlistsources")]
        public string PlaylistSources { get; set; }

        [Map("startnumberframe")]
        public int startNumberBuffer { get; set; } = 0;

        [Map("endnumberframe")]
        public int endNumberBuffer { get; set; }

        //convert EpisodeDTO to Episode
        public Episode EpisodeDTOToEpisode(EpisodeDTO episode)
        {
            return new Episode
            {
                ID = episode.ID,
                AnimeId = episode.AnimeId,
                UrlVideo = episode.UrlVideo,
                NumberEpisodeCurrent = episode.NumberEpisodeCurrent,
                NumberSeasonCurrent = episode.NumberSeasonCurrent,
                StateDownload = episode.StateDownload,
                PercentualDownload = episode.PercentualDownload,
                startNumberBuffer = episode.startNumberBuffer,
                endNumberBuffer = episode.endNumberBuffer,
                Resolution = episode.Resolution,
                PlaylistSources = episode.PlaylistSources,
                Playlist = episode.Playlist,
                BaseUrl = episode.BaseUrl
            };
        }
    }
}
