using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.NotifyService
{
    public class Worker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(60000, stoppingToken);
            }
        }
    }
}