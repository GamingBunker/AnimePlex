using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Cesxhin.AnimeSaturn.Application.Consumers;
using System;
using NLog;
using Cesxhin.AnimeSaturn.Application.Generic;
using Quartz;
using Cesxhin.AnimeSaturn.Application.CronJob;

namespace Cesxhin.AnimeSaturn.DownloadService
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

                            cfg.ReceiveEndpoint("download-anime", e => {
                                e.Consumer<DownloadAnimeConsumer>(cc =>
                                {
                                    string limit = Environment.GetEnvironmentVariable("LIMIT_CONSUMER_RABBIT") ?? "3";

                                    cc.UseConcurrentMessageLimit(int.Parse(limit));
                                });
                            });

                            cfg.ReceiveEndpoint("download-manga", e => {
                                e.Consumer<DownloadMangaConsumer>(cc =>
                                {
                                    string limit = Environment.GetEnvironmentVariable("LIMIT_CONSUMER_RABBIT") ?? "3";

                                    cc.UseConcurrentMessageLimit(int.Parse(limit));
                                });
                            });

                        });
                    });

                    //setup nlog
                    var level = Environment.GetEnvironmentVariable("LOG_LEVEL").ToLower() ?? "info";
                    LogLevel logLevel = NLogManager.GetLevel(level);
                    NLogManager.Configure(logLevel);

                    //cronjob for check health
                    services.AddQuartz(q =>
                    {
                        q.UseMicrosoftDependencyInjectionJobFactory();
                        q.ScheduleJob<HealthJob>(trigger => trigger
                            .StartNow()
                            .WithDailyTimeIntervalSchedule(x => x.WithIntervalInSeconds(60)), job => job.WithIdentity("download"));
                    });
                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

                    services.AddHostedService<Worker>();
                });
    }
}
