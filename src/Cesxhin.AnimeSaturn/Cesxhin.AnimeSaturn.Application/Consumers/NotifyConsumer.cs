using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using Discord.Webhook;
using MassTransit;
using NLog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Consumers
{
    public class NotifyConsumer : IConsumer<NotifyDTO>
    {
        //nlog
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //webhook discord
        private readonly string _webhookDiscord = Environment.GetEnvironmentVariable("WEBHOOK_DISCORD");

        public Task Consume(ConsumeContext<NotifyDTO> context)
        {
            DiscordWebhookClient discord = new(_webhookDiscord);

            var notify = context.Message;
            _logger.Info($"Recive this message: {notify.Message}");

            try
            {
                if(notify.Image != null)
                {
                    Stream image = new MemoryStream(notify.Image);
                    discord.SendFileAsync(image, "Cover.png", notify.Message).GetAwaiter().GetResult();
                }
                else
                    discord.SendMessageAsync(notify.Message).GetAwaiter().GetResult();
                _logger.Info("Ok send done!");
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"error send webhook to discord, details error: {ex.Message}");
            }

            return Task.CompletedTask;
        }
    }
}
