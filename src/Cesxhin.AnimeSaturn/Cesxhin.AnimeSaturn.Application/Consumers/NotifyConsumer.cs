using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using MassTransit;
using NLog;
using System;
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
            //api
            Api<MessageDiscord> discordApi = new();

            var notify = context.Message;
            _logger.Info($"Recive this message: {notify.Message}");

            var data = new MessageDiscord { 
                content = notify.Message 
            };

            try
            {
                discordApi.PostMessageDiscord(_webhookDiscord, data);
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
