using Cesxhin.AnimeSaturn.Application.Consumers;
using Cesxhin.AnimeSaturn.Application.Generic;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System;

namespace Cesxhin.AnimeSaturn.NotifyService
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
                                Environment.GetEnvironmentVariable("ADDRESS_RABBIT"),
                                "/",
                                credentials =>
                                {
                                    credentials.Username(Environment.GetEnvironmentVariable("USERNAME_RABBIT"));
                                    credentials.Password(Environment.GetEnvironmentVariable("PASSWORD_RABBIT"));
                                });
                            cfg.ReceiveEndpoint("notify-anime", e => {
                                e.Consumer<NotifyConsumer>();
                            });

                        });
                    });
                    services.AddMassTransitHostedService();

                    //setup nlog
                    var level = Environment.GetEnvironmentVariable("LOG_LEVEL").ToLower() ?? "info";
                    LogLevel logLevel = NLogManager.GetLevel(level);
                    NLogManager.Configure(logLevel);

                    services.AddHostedService<Worker>();
                });
    }
}
