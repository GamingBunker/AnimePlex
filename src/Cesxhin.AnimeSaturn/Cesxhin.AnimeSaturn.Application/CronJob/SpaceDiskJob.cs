using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.CronJob
{
    public class SpaceDiskJob : IJob
    {
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";
        public Task Execute(IJobExecutionContext context)
        {
            Api<SpaceDiskDTO> checkDiskFreeSpaceApi = new Api<SpaceDiskDTO>();
            //check disk space free
            var freeGigabytes = new DriveInfo(_folder).AvailableFreeSpace / 1000000000;
            var totalGigabytes = new DriveInfo(_folder).TotalSize / 1000000000;

            var disk = new SpaceDiskDTO
            {
                DiskSizeFree = freeGigabytes,
                DiskSizeTotal = totalGigabytes,
            };

            checkDiskFreeSpaceApi.PutOne("/disk", disk).GetAwaiter().GetResult();

            return Task.CompletedTask;
        }
    }
}
