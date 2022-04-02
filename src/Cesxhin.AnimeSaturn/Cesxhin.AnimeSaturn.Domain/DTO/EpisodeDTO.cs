using Cesxhin.AnimeSaturn.Domain.Models;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class EpisodeDTO
    {
        public int ID { get; set; }
        public string AnimeId { get; set; }
        public string UrlVideo { get; set; }
        public int NumberEpisodeCurrent { get; set; }
        public int NumberSeasonCurrent { get; set; }
        public string StateDownload { get; set; }
        public int PercentualDownload { get; set; }

        /*
         Download playlist: https://www.saturnspeed49.org/DDL/ANIME/HatarakuSaibou2/01/playlist.m3u8
         Select Resolution for download source: https://www.saturnspeed49.org/DDL/ANIME/HatarakuSaibou2/01/360p/playlist_360p.m3u8
         Download source: https://www.saturnspeed49.org/DDL/ANIME/HatarakuSaibou2/01/360p/360p-000.ts
         */

        //alternative source
        public string BaseUrl { get; set; }
        public string Playlist { get; set; }
        public string Resolution { get; set; }
        public string PlaylistSources { get; set; }
        public int startNumberBuffer { get; set; } = 0;
        public int endNumberBuffer { get; set; }

        public static EpisodeDTO EpisodeToEpisodeDTO(Episode episode)
        {
            return new EpisodeDTO
            {
                ID = episode.ID,
                AnimeId = episode.AnimeId,
                UrlVideo = episode.UrlVideo,
                NumberEpisodeCurrent = episode.NumberEpisodeCurrent,
                NumberSeasonCurrent = episode.NumberSeasonCurrent,
                StateDownload = episode.StateDownload,
                PercentualDownload = episode.PercentualDownload,
                BaseUrl = episode.BaseUrl,
                Playlist = episode.Playlist,
                Resolution = episode.Resolution,
                PlaylistSources = episode.PlaylistSources,
                startNumberBuffer = episode.startNumberBuffer,
                endNumberBuffer = episode.endNumberBuffer
            };
        }
    }
}
