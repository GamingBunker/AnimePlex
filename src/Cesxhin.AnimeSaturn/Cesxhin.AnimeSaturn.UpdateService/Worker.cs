using Cesxhin.AnimeSaturn.Application.CheckManager.Interfaces;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.UpdateService
{
    public class Worker : BackgroundService
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //timer
        private readonly int _timeRefresh = int.Parse(Environment.GetEnvironmentVariable("TIME_REFRESH") ?? "120000");

        //service
        private readonly IUpdate _updateAnime;

        public Worker(IUpdate updateAnime)
        {
            _updateAnime = updateAnime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _updateAnime.ExecuteUpdate();
                }catch (Exception ex)
                {
                    _logger.Fatal($"Error updateAnime, details error: {ex}");
                }

                _logger.Info($"Worker running at: {DateTimeOffset.Now}");
                await Task.Delay(_timeRefresh, stoppingToken);
            }
        }
    }
}
