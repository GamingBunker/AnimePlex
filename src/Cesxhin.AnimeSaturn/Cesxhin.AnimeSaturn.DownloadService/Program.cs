using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Cesxhin.AnimeSaturn.Application.Consumers;
using System;

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
                                Environment.GetEnvironmentVariable("ADDRESS_RABBIT"),
                                "/",
                                credentials =>
                                {
                                    credentials.Username(Environment.GetEnvironmentVariable("USERNAME_RABBIT"));
                                    credentials.Password(Environment.GetEnvironmentVariable("PASSWORD_RABBIT"));
                                });
                            cfg.ReceiveEndpoint("download-anime", e => {
                                e.Consumer<DownloadConsumer>(cc =>
                                {
                                    string limit = Environment.GetEnvironmentVariable("LIMIT_CONSUMER_RABBIT") ?? "3";

                                    cc.UseConcurrentMessageLimit(int.Parse(limit));
                                });
                            });

                        });
                    });
                    services.AddMassTransitHostedService();
                    services.AddHostedService<Worker>();
                });
    }
}
