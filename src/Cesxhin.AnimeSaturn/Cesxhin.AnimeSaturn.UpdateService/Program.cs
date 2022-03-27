using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using System;

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
                                Environment.GetEnvironmentVariable("ADDRESS_RABBIT"),
                                "/",
                                credentials =>
                                {
                                    credentials.Username(Environment.GetEnvironmentVariable("USERNAME_RABBIT"));
                                    credentials.Password(Environment.GetEnvironmentVariable("PASSWORD_RABBIT"));
                                });
                        });
                    });
                    services.AddMassTransitHostedService();
                    services.AddHostedService<Worker>();
                });
    }
}
