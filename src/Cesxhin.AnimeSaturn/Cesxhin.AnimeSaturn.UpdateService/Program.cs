using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using System;
using Cesxhin.AnimeSaturn.Application.Generic;
using NLog;
using Cesxhin.AnimeSaturn.Application.CronJob;
using Quartz;
using Cesxhin.AnimeSaturn.Application.CheckManager.Interfaces;
using Cesxhin.AnimeSaturn.Application.CheckManager;

namespace Cesxhin.AnimeSaturn.UpdateService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    //rabbit
                    services.AddMassTransit(
                    x =>
                    {
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(
                                Environment.GetEnvironmentVariable("ADDRESS_RABBIT") ?? "localhost",
                                "/",
                                credentials =>
                                {
                                    credentials.Username(Environment.GetEnvironmentVariable("USERNAME_RABBIT") ?? "guest");
                                    credentials.Password(Environment.GetEnvironmentVariable("PASSWORD_RABBIT") ?? "guest");
                                });
                        });
                    });

                    //setup nlog
                    var level = Environment.GetEnvironmentVariable("LOG_LEVEL").ToLower() ?? "info";
                    LogLevel logLevel = NLogManager.GetLevel(level);
                    NLogManager.Configure(logLevel);

                    //select service between anime or manga
                    var serviceSelect = Environment.GetEnvironmentVariable("SELECT_SERVICE") ?? "anime";

                    //cronjob for check health
                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();
                        q.ScheduleJob<HealthJob>(trigger => trigger
                            .StartNow()
                            .WithDailyTimeIntervalSchedule(x => x.WithIntervalInSeconds(60)), job => job.WithIdentity("update-"+ serviceSelect));

                        q.ScheduleJob<SpaceDiskJob>(trigger => trigger
                            .StartNow()
                            .WithDailyTimeIntervalSchedule(x => x.WithIntervalInSeconds(60)));
                    });
                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

                    if(serviceSelect.ToLower().Contains("anime"))
                        services.AddTransient<IUpdate, UpdateAnime>();
                    else
                        services.AddTransient<IUpdate, UpdateManga>();

                    services.AddHostedService<Worker>();
                });
    }
}
