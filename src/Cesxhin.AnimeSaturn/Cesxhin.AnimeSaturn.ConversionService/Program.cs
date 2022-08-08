using Cesxhin.AnimeSaturn.Application.Consumers;
using Cesxhin.AnimeSaturn.Application.CronJob;
using Cesxhin.AnimeSaturn.Application.Generic;
using FFMpegCore;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using Quartz;
using System;

namespace Cesxhin.AnimeSaturn.ConversionService
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

                            cfg.ReceiveEndpoint("conversion", e => {
                                e.Consumer<ConversionConsumer>(cc =>
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
                            .WithDailyTimeIntervalSchedule(x => x.WithIntervalInSeconds(60)), job => job.WithIdentity("conversion"));
                    });
                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

                    //set ffmpeg
                    GlobalFFOptions.Configure(options => options.BinaryFolder = Environment.GetEnvironmentVariable("PATH_FFMPEG").ToLower() ?? "./bin");

                    services.AddHostedService<Worker>();
                });
    }
}
