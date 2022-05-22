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
                _logger.Info($"Start download manga {chapter.NameManga} of volume {chapter.CurrentVolume} chapter {chapter.CurrentChapter}");

                //create empty file
                for (int i = 0; i <= chapter.NumberMaxImage; i++)
                {
                    //check directory
                    var pathWithoutFile = Path.GetDirectoryName(chapterRegister.ChapterPath[i]);
                    if (Directory.Exists(pathWithoutFile) == false)
                        Directory.CreateDirectory(pathWithoutFile);

                    File.WriteAllBytes(chapterRegister.ChapterPath[i], new byte[0]);
                }


                //set start download
                chapter.StateDownload = "downloading";
                SendStatusDownloadAPIAsync(chapter, chapterApi);

                //start
                for (int i=0; i<= chapter.NumberMaxImage; i++)
                {
                    var imgBytes = HtmlMangaMangaWorld.GetImagePage(chapter.UrlPage, i+1);

                    File.WriteAllBytes(chapterRegister.ChapterPath[i], imgBytes);

                    //send status
                    chapter.PercentualDownload = (100*i)/chapter.NumberMaxImage;
                    SendStatusDownloadAPIAsync(chapter, chapterApi);

                }
            }

            //end download
            chapter.PercentualDownload = 100;
            chapter.StateDownload = "completed";
            SendStatusDownloadAPIAsync(chapter, chapterApi);

            //get hash and update
            _logger.Info($"start calculate hash of chapter id: {chapter.ID}");
            List<string> listHash = new();
            for (int i = 0; i <= chapter.NumberMaxImage; i++)
            {
                listHash.Add(Hash.GetHash(chapterRegister.ChapterPath[i]));
            }
            _logger.Info($"end calculate hash of episode id: {chapter.ID}");

            chapterRegister.ChapterHash = listHash.ToArray();

            try
            {
                chapterRegisterApi.PutOne("/chapter/register", chapterRegister).GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found episodeRegister id: {chapterRegister.ChapterId}, details error: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"Error generic put episodeRegister, details error: {ex.Message}");
            }

            _logger.Info($"Done download manga {chapter.NameManga} of volume {chapter.CurrentVolume} chapter {chapter.CurrentChapter}");
            return Task.CompletedTask;
        }

        private void SendStatusDownloadAPIAsync(ChapterDTO chapter, Api<ChapterDTO> chapterApi)
        {
            try
            {
                chapterApi.PutOne("/manga/statusDownload", chapter).GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found episode id: {chapter.ID}, details: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Error($"Error generic api, details: {ex.Message}");
            }
        }
    }
}
