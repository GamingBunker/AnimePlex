using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using MassTransit;
using NLog;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Consumers
{
    public class NotifyConsumer : IConsumer<NotifyDTO>
    {
        //nlog
        Logger logger = LogManager.GetCurrentClassLogger();

        //webhook discord
        private readonly string webhookDiscord = Environment.GetEnvironmentVariable("WEBHOOK_DISCORD");

        public Task Consume(ConsumeContext<NotifyDTO> context)
        {
            var notify = context.Message;
            logger.Debug($"Recive this message: {notify.Message}");

            var data = new MessageDiscord { 
                content = notify.Message 
            };

            using (var client = new HttpClient())
            using (var content = new StringContent(JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json"))
            {
                try
                {
                    client.PostAsync(webhookDiscord, content).Wait();
                }catch (Exception ex)
                {
                    logger.Error("Cannot send notify to discord, error detials: "+ex.Message);
                }
            }
            return Task.CompletedTask;
        }
    }
}
