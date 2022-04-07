using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Domain.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Cesxhin.AnimeSaturn.Application.NlogManager
{
    public class NLogConsole
    {
        private readonly Logger _logger;
        private readonly string webhookUrl;
        private readonly Api<MessageDiscord> webhookApi;

        public NLogConsole(Logger logger)
        {
            //log
            _logger = logger;

            //webhook
            webhookUrl = Environment.GetEnvironmentVariable("WEBHOOK_DISCORD_DEBUG") ?? null;

            //api
            webhookApi = new Api<MessageDiscord>();
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

            if (webhookUrl != null)
            {
                var _content = DefaultMessage(LogLevel.Info, msg.ToString());

                webhookApi.PostMessageDiscord(webhookUrl, new MessageDiscord
                {
                    content = _content,
                });
            }
        }

        //warn
        public void Warn(object msg)
        {
            _logger.Warn(msg);

            if (webhookUrl != null)
            {
                var _content = DefaultMessage(LogLevel.Warn, msg.ToString());

                webhookApi.PostMessageDiscord(webhookUrl, new MessageDiscord
                {
                    content = _content
                });
            }
        }

        //error
        public void Error(object msg)
        {
            _logger.Error(msg);

            if (webhookUrl != null)
            {
                var _content = DefaultMessage(LogLevel.Error, msg.ToString());

                webhookApi.PostMessageDiscord(webhookUrl, new MessageDiscord
                {
                    content = _content
                });
            }
        }

        //fatal
        public void Fatal(object msg)
        {
            _logger.Fatal(msg);

            if (webhookUrl != null)
            {
                var _content = DefaultMessage(LogLevel.Fatal, msg.ToString());

                webhookApi.PostMessageDiscord(webhookUrl, new MessageDiscord
                {
                    content = _content
                });
            }
        }
    }
}
