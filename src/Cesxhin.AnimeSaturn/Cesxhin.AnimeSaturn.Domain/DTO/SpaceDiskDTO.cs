using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class SpaceDiskDTO
    {
        public long DiskSizeTotal { get; set; }
        public long DiskSizeFree { get; set; }
        public int Interval { get; set; }
        public long LastCheck { get; set; }
    }
}
