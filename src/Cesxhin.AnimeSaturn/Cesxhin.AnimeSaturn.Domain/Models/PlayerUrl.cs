using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Domain.Models
{
    public class PlayerUrl
    {
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
    }
}
