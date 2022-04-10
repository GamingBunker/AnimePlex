using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class HealthDTO
    {
        public string NameService { get; set; }
        public long LastCheck { get; set; }
        public int Interval { get; set; }
    }
}
