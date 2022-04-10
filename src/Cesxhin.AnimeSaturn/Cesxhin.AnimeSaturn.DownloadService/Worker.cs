using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.DownloadService
{
    public class Worker : BackgroundService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Api<HealthDTO> healtApi = new Api<HealthDTO>();
            while (!stoppingToken.IsCancellationRequested)
            {
                //logger.Info("Worker running at: {time}", DateTimeOffset.Now);
                /*healtApi.PutOne(new HealthServiceDTO
                {
                    nameService = "download",
                    time = 
                });*/
                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}
