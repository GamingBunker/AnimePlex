using Cesxhin.AnimeSaturn.Application.CheckManager.Interfaces;
using Cesxhin.AnimeSaturn.Application.Exceptions;
using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.HtmlAgilityPack;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cesxhin.AnimeSaturn.Application.CheckManager
{
    public class UpgradeManga : IUpgrade
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //variables
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";

        //rabbit
        private readonly IBus _publishEndpoint;

        public UpgradeManga(IBus publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }
        public void ExecuteUpgrade()
        {
            //list
            List<GenericMangaDTO> listGenerics = new();
            List<ChapterRegisterDTO> listChapterRegister;
            List<ChapterRegisterDTO> blacklist;

            //api
            Api<GenericMangaDTO> genericApi = new();
            Api<ChapterDTO> chapterApi = new();
            Api<ChapterRegisterDTO> chapterRegisterApi = new();

            try
            {
                listGenerics = genericApi.GetMore("/manga/all").GetAwaiter().GetResult();
            }
            catch (ApiNotFoundException ex)
            {
                _logger.Error($"not found, details: " + ex.Message);
            }

            //step check on website if the anime is still active
            foreach (var list in listGenerics)
            {
                var manga = list.Manga;

                //get list episodes by name
                List<ChapterDTO> checkChapters = null;
                List<ChapterDTO> listChaptersAdd = null;

                _logger.Info("Check new episodes for manga: " + manga.Name);

                //check new episode
                var doc = HtmlMangaMangaWorld.GetMangaHtml(manga.UrlPage);
                checkChapters = HtmlMangaMangaWorld.GetChapters(doc, manga.UrlPage, manga);

                //check if null
                if (checkChapters == null)
                {
                    _logger.Error($"Can't download with this url, {manga.UrlPage}");
                    continue;
                }

                listChaptersAdd = new(checkChapters);
                blacklist = new();

                foreach (var checkChapter in checkChapters)
                {
                    foreach (var chapter in list.Chapters)
                    {
                        if (chapter.CurrentChapter == checkChapter.CurrentChapter)
                        {
                            blacklist.Add(list.ChapterRegister.Find(e => e.ChapterId == chapter.ID));
                            listChaptersAdd.Remove(checkChapter);
                            break;
                        }
                    }
                }

                if (listChaptersAdd.Count > 0)
                {
                    _logger.Info($"There are new chapters ({listChaptersAdd.Count}) of {manga.Name}");

                    //insert to db
                    listChaptersAdd = chapterApi.PostMore("/chapters", listChaptersAdd).GetAwaiter().GetResult();

                    //create episodeRegister
                    listChapterRegister = new();

                    string pathDefault = null;
                    List<string> paths = new();

                    if (blacklist.Count > 0)
                        pathDefault = Path.GetDirectoryName(blacklist.FirstOrDefault().ChapterPath.First());

                    foreach (var chapter in listChaptersAdd)
                    {
                        //use path how others episodesRegisters
                        for(int i=0; i<=chapter.NumberMaxImage; i++)
                        {
                            if (pathDefault != null)
                            {
                                paths.Add($"{pathDefault}/{chapter.NameManga}/Volume {chapter.CurrentVolume}/Chapter {chapter.CurrentChapter}/{chapter.NameManga} s{chapter.CurrentVolume}c{chapter.CurrentChapter}n{i}.png");
                            }
                            else //default
                            {
                                paths.Add($"{_folder}/{chapter.NameManga}/Volume {chapter.CurrentVolume}/Chapter {chapter.CurrentChapter}/{chapter.NameManga} s{chapter.CurrentVolume}c{chapter.CurrentChapter}n{i}.png");
                            }
                        }

                        listChapterRegister.Add(new ChapterRegisterDTO
                        {
                            ChapterId = chapter.ID,
                            ChapterPath = paths.ToArray()
                        });

                        paths.Clear();
                    }

                    chapterRegisterApi.PostMore("/chapters/registers", listChapterRegister).GetAwaiter();

                    //create message for notify
                    string message = $"💽UpgradeService say: \nAdd new chapter of {manga.Name}\n";

                    listChaptersAdd.Sort(delegate (ChapterDTO p1, ChapterDTO p2) { return p1.CurrentChapter.CompareTo(p2.CurrentChapter); });
                    foreach (var episodeNotify in listChaptersAdd)
                    {
                        message += $"- {episodeNotify.ID} Chapter: {episodeNotify.CurrentChapter}\n";
                    }

                    try
                    {
                        var messageNotify = new NotifyDTO
                        {
                            Message = message,
                            Image = manga.Image
                        };
                        _publishEndpoint.Publish(messageNotify).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
                    }


                    _logger.Info($"Done upgrade! of {manga.Name}");
                }
                //clear resource
                listChaptersAdd.Clear();
            }
        }
    }
}
