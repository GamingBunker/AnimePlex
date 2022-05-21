using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.HtmlAgilityPack;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Application.Parallel;
using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Consumers
{
    public class DownloadMangaConsumer : IConsumer<ChapterDTO>
    {
        //nlog
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        public Task Consume(ConsumeContext<ChapterDTO> context)
        {
            //get body
            var chapter = context.Message;

            //api
            Api<ChapterDTO> chapterApi = new();
            Api<ChapterRegisterDTO> chapterRegisterApi = new();

            //register
            ChapterRegisterDTO chapterRegister = null;
            try
            {
                chapterRegister = chapterRegisterApi.GetOne($"/chapter/register/chapterid/{chapter.ID}").GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found episodeRegister, details error: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"Impossible error generic get episodeRegister, details error: {ex.Message}");
            }

            //episode
            ChapterDTO chapterVerify = null;
            try
            {
                chapterVerify = chapterApi.GetOne($"/chapter/id/{chapter.ID}").GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found episodeRegister, details error: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"Impossible error generic get episodeRegister, details error: {ex.Message}");
            }

            //check duplication messages
            if (chapterVerify != null && chapterVerify.StateDownload == "pending")
            {
                for(int i=0; i<chapter.NumberMaxImage; i++)
                {
                    var imgBytes = HtmlMangaMangaWorld.GetImagePage(chapter.UrlPage, i+1);

                    var pathWithoutFile = Path.GetDirectoryName(chapterRegister.ChapterPath[i]);
                    if (Directory.Exists(pathWithoutFile) == false)
                        Directory.CreateDirectory(pathWithoutFile);

                    File.WriteAllBytes(chapterRegister.ChapterPath[i], imgBytes);
                }
            }
            _logger.Info($"Done download manga {chapter.NameManga} of volume {chapter.CurrentVolume} chapter {chapter.CurrentChapter}");
            return Task.CompletedTask;
        }
    }
}
