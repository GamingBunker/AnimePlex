using Cesxhin.AnimeSaturn.Application.NlogManager;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.NotifyService
{
    public class Worker : BackgroundService
    {
        //nlog
        private readonly NLogConsole logger = new NLogConsole(LogManager.GetCurrentClassLogger());

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //logger.Info($"Worker running at: {DateTimeOffset.Now}");
                await Task.Delay(1200000, stoppingToken);
            }
        }
    }
}