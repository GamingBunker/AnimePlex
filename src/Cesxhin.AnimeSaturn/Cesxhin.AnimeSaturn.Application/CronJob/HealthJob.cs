using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.DTO;
using NLog;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.CronJob
{
    public class HealthJob : IJob
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        public Task Execute(IJobExecutionContext context)
        {
            Api<HealthDTO> api = new();

            try
            {
                api.PutOne("/health", new HealthDTO
                {
                    NameService = context.JobDetail.Key.Name.ToLower(),
                    LastCheck = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds(),
                    Interval = 60000
                }).GetAwaiter().GetResult();

            }catch (ApiGenericException ex)
            {
                _logger.Fatal($"Error api, error details: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
