using Cesxhin.AnimeSaturn.Application.CheckManager.Interfaces;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.UpgradeService
{
    public class Worker : BackgroundService
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //variables
        private readonly int _timeRefresh = int.Parse(Environment.GetEnvironmentVariable("TIME_REFRESH") ?? "1200000");

        //services
        private readonly IUpgrade _upgrade;

        public Worker(IUpgrade upgrade)
        {
            _upgrade = upgrade;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _upgrade.ExecuteUpgrade();
                }catch (Exception ex)
                {
                    _logger.Fatal($"Error upgradeAnime, details error: {ex}");
                }

                _logger.Info($"Worker running at: {DateTimeOffset.Now}");
                await Task.Delay(_timeRefresh, stoppingToken);
            }
        }
    }
}
