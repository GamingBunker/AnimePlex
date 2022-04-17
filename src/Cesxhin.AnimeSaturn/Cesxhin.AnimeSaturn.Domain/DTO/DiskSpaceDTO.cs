namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class DiskSpaceDTO
    {
        public long DiskSizeTotal { get; set; }
        public long DiskSizeFree { get; set; }
        public int Interval { get; set; }
        public long LastCheck { get; set; }
    }
}
