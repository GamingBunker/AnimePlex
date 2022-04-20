using Discord.Webhook;
using NLog;
using System;

namespace Cesxhin.AnimeSaturn.Application.NlogManager
{
    public class NLogConsole
    {
        private readonly Logger _logger;
        private readonly DiscordWebhookClient discord;

        public NLogConsole(Logger logger)
        {
            //log
            _logger = logger;

            //webhook
            var webhookUrl = Environment.GetEnvironmentVariable("WEBHOOK_DISCORD_DEBUG") ?? null;

            if(webhookUrl != null)
                discord = new DiscordWebhookClient(webhookUrl);
        }

        private string DefaultMessage(LogLevel level,  string msg)
        {
            string message = "";
            message += DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + " | ";

            //set type error
            switch(level.Name.ToLower())
            {
                case "info":
                    message += "🔵 info: ";
                    break;
                case "warn":
                    message += "🔶 warning: ";
                    break;
                case "error":
                    message += "🔴 error: ";
                    break;
                case "fatal":
                    message += "💀 fatal: ";
                    break;
            }
            message += msg;

            return message;
        }

        //debug
        public void Debug(object msg)
        {
            _logger.Debug(msg);
        }

        //info
        public void Info(object msg)
        {
            _logger.Info(msg);

            if (discord != null)
            {
                var _content = DefaultMessage(LogLevel.Info, msg.ToString());

                discord.SendMessageAsync(_content).GetAwaiter().GetResult();
            }
        }

        //warn
        public void Warn(object msg)
        {
            _logger.Warn(msg);

            if (discord != null)
            {
                var _content = DefaultMessage(LogLevel.Warn, msg.ToString());

                discord.SendMessageAsync(_content).GetAwaiter().GetResult();
            }
        }

        //error
        public void Error(object msg)
        {
            _logger.Error(msg);

            if (discord != null)
            {
                var _content = DefaultMessage(LogLevel.Error, msg.ToString());

                discord.SendMessageAsync(_content).GetAwaiter().GetResult();
            }
        }

        //fatal
        public void Fatal(object msg)
        {
            _logger.Fatal(msg);

            if (discord != null)
            {
                var _content = DefaultMessage(LogLevel.Fatal, msg.ToString());

                discord.SendMessageAsync(_content).GetAwaiter().GetResult();
            }
        }
    }
}
