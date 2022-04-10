using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.DTO;
using NLog;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.CronJob
{
    public class HealthJob : IJob
    {
        //log
        private readonly NLogConsole logger = new NLogConsole(LogManager.GetCurrentClassLogger());
        public Task Execute(IJobExecutionContext context)
        {
            //logger.Debug($"Hey! I'm {context.JobDetail.Key.Name} {DateTime.Now}");
            Api<HealthDTO> api = new Api<HealthDTO>();
            api.PutOne("/health", new HealthDTO
            {
                NameService = context.JobDetail.Key.Name.ToLower(),
                LastCheck = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds(),
                Interval = 60000
            }).GetAwaiter().GetResult();
            return Task.CompletedTask;
        }
    }
}
