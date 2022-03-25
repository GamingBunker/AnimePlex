using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class EpisodeDTO
    {
        public int ID { get; set; }
        public string IDAnime { get; set; }
        public string UrlVideo { get; set; }
        public string Referer { get; set; }
        public int NumberEpisodeCurrent { get; set; }
        public int NumberSeasonCurrent { get; set; }
        public string StateDownload { get; set; }
        public int PercentualDownload { get; set; }
        public int SizeFile { get; set; }

        //alternative source
        /*
         Download playlist: https://www.saturnspeed49.org/DDL/ANIME/HatarakuSaibou2/01/playlist.m3u8
         Select Resolution for download source: https://www.saturnspeed49.org/DDL/ANIME/HatarakuSaibou2/01/360p/playlist_360p.m3u8
         Download source: https://www.saturnspeed49.org/DDL/ANIME/HatarakuSaibou2/01/360p/360p-000.ts
         */
        public string BaseUrl { get; set; }
        public string Playlist { get; set; }
        public string Resolution { get; set; }
        public string PlaylistSources { get; set; }
        public int startNumberBuffer { get; set; } = 0;
        public int endNumberBuffer { get; set; }

        public EpisodeDTO EpisodeToEpisodeDTO(Episode episode)
        {
            return new EpisodeDTO
            {
                ID = episode.ID,
                IDAnime = episode.IDAnime,
                UrlVideo = episode.UrlVideo,
                NumberEpisodeCurrent = episode.NumberEpisodeCurrent,
                Referer = episode.Referer,
                NumberSeasonCurrent = episode.NumberSeasonCurrent,
                StateDownload = episode.StateDownload,
                PercentualDownload = episode.PercentualDownload,
                SizeFile = episode.SizeFile,
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
