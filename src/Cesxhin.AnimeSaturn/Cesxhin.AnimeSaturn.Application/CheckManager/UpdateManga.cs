using Cesxhin.AnimeSaturn.Application.CheckManager.Interfaces;
using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Application.Parallel;
using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;

namespace Cesxhin.AnimeSaturn.Application.CheckManager
{
    public class UpdateManga : IUpdate
    {
        //interface
        private readonly IBus _publishEndpoint;

        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";

        //Instance Parallel
        private readonly ParallelManager<object> parallel = new();

        //Istance Api
        private readonly Api<GenericMangaDTO> mangaApi = new();
        private readonly Api<ChapterDTO> chapterApi = new();
        private readonly Api<ChapterRegisterDTO> chapterRegisterApi = new();

        public UpdateManga(IBus publicEndpoint)
        {
            _publishEndpoint = publicEndpoint;
        }

        public void ExecuteUpdate()
        {
            //download api
            List<GenericMangaDTO> listManga = null;
            parallel.ClearList();

            try
            {
                listManga = mangaApi.GetMore("/manga/all").GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Not found get all, details error: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"Error generic get all, details error: {ex.Message}");
            }

            //if exists listManga
            if (listManga != null)
            {
                var tasks = new List<Func<object>>();
                //step one check file
                foreach (var manga in listManga)
                {

                    //foreach chapters
                    foreach (var chapter in manga.Chapters)
                    {
                        tasks.Add(new Func<object>(() => Checkchapter(manga, chapter, chapterApi, chapterRegisterApi)));
                    }
                }
                parallel.AddTasks(tasks);
                parallel.Start();
                parallel.WhenCompleted();
                parallel.ClearList();
            }
        }

        private object Checkchapter(GenericMangaDTO manga, ChapterDTO chapter, Api<ChapterDTO> chapterApi, Api<ChapterRegisterDTO> chapterRegisterApi)
        {
            var chapterRegister = manga.ChapterRegister.Find(e => e.ChapterId == chapter.ID);
            if (chapterRegister == null)
            {
                _logger.Warn($"not found chapterRegister by chapter id: {chapter.ID}");
                return null;
            }

            for(int i=0; i<chapterRegister.ChapterPath.Length; i++)
            {
                _logger.Debug($"check {chapterRegister.ChapterPath[i]}");

                //check integry file
                if (chapter.StateDownload == null || chapter.StateDownload == "failed" || (chapter.StateDownload == "completed" && chapterRegister.ChapterHash == null))
                {
                    ConfirmStartDownloadAnime(chapter, chapterApi);
                }
                else if ((!File.Exists(chapterRegister.ChapterPath[i]) && chapter.StateDownload != "pending"))
                {
                    var found = false;
                    string newHash;

                    foreach (string file in Directory.EnumerateFiles(_folder, "*.png", SearchOption.AllDirectories))
                    {
                        newHash = Hash.GetHash(file);
                        if (newHash == chapterRegister.ChapterHash[i])
                        {
                            _logger.Info($"I found file (chapter id: {chapter.ID}) that was move, now update information");

                            //update
                            chapterRegister.ChapterPath[i] = file;
                            try
                            {
                                chapterRegisterApi.PutOne("/chapter/register", chapterRegister).GetAwaiter().GetResult();

                                _logger.Info($"Ok update chapter id: {chapter.ID} that was move");

                                //return
                                found = true;
                            }
                            catch (ApiNotFoundException ex)
                            {
                                _logger.Error($"Not found chapterRegister id: {chapterRegister.ChapterId} for update information, details: {ex.Message}");
                            }
                            catch (ApiConflictException ex)
                            {
                                _logger.Error($"Error conflict put chapterRegister, details error: {ex.Message}");
                            }
                            catch (ApiGenericException ex)
                            {
                                _logger.Fatal($"Error generic put chapterRegister, details error: {ex.Message}");
                            }

                            break;
                        }
                    }

                    //if not found file
                    if (found == false)
                        ConfirmStartDownloadAnime(chapter, chapterApi);
                }
            }

            return null;
        }

        private async void ConfirmStartDownloadAnime(ChapterDTO chapter, Api<ChapterDTO> chapterApi)
        {
            //set pending to 
            chapter.StateDownload = "pending";

            try
            {
                //set change status
                await chapterApi.PutOne("/manga/statusDownload", chapter);

                await _publishEndpoint.Publish(chapter);
                _logger.Info($"this file ({chapter.NameManga} volume: {chapter.CurrentVolume} chapter: {chapter.CurrentChapter}) does not exists, sending message to DownloadService");
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"Impossible update chapter becouse not found chapter id: {chapter.ID}, details: {ex.Message}");
            }
            catch (ApiGenericException ex)
            {
                _logger.Fatal($"Error update chapter, details error: {ex.Message}");
            }
        }
    }
}
