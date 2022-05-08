using Cesxhin.AnimeSaturn.Application.HtmlAgilityPack;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.DTO;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class MangaController : ControllerBase
    {
        //interfaces
        private readonly IMangaService _mangaService;
        private readonly IChapterService _chapterService;
        private readonly IBus _publishEndpoint;

        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";

        public MangaController(
            IMangaService mangaService,
            IChapterService chapterService,
            IBus publishEndpoint
            )
        {
            _publishEndpoint = publishEndpoint;
            _mangaService = mangaService;
            _chapterService = chapterService;
        }

        [HttpPost("/manga/download")]
        public async Task<IActionResult> GetManga(DownloadDTO download)
        {
            var urlPage = download.Url;

            //download page
            var html = HtmlMangaMangaWorld.GetMangaHtml(urlPage);

            //get manga
            var manga = HtmlMangaMangaWorld.GetManga(html, urlPage);

            //insert manga
            manga = await _mangaService.InsertMangaAsync(manga);

            if (manga == null)
                return Conflict();

            //get chapters
            var chapters = HtmlMangaMangaWorld.GetChapters(html, urlPage, manga);

            //insert chapters
            var listChapters = await _chapterService.InsertChaptersAsync(chapters);

            if (listChapters == null)
                return Conflict();

            //create message for notify
            string message = $"🧮ApiService say: \nAdd new Manga: {manga.Name}\n";

            try
            {
                var messageNotify = new NotifyDTO
                {
                    Message = message,
                    Image = manga.Image
                };
                await _publishEndpoint.Publish(messageNotify);
            }
            catch (Exception ex)
            {
                _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
            }

            return Created("none", manga);
        }

        [HttpGet("/manga/list/name/{name}")]
        public async Task<IActionResult> GetManga(string name)
        {
            try
            {
                var list = HtmlMangaMangaWorld.GetMangaUrl(name);
                if (list != null)
                {
                    //list anime
                    List<GenericUrlDTO> listManga = new();

                    foreach (var manga in list)
                    {
                        var mangaDTO = GenericUrlDTO.GenericUrlToGenericUrlDTO(manga);

                        var checkManga = await _mangaService.GetMangaByName(manga.Name);
                        if (checkManga != null)
                            mangaDTO.Exists = true;

                        listManga.Add(mangaDTO);
                    }
                    return Ok(listManga);
                }
                return NotFound();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpDelete("/manga/{name}")]
        public async Task<IActionResult> DeleteManga(string name)
        {
            try
            {
                var manga = await _mangaService.DeleteMangaByNameAsync(name);

                if(manga == null)
                    return NotFound();

                //create message for notify
                string message = $"🧮ApiService say: \nRemoved this Manga by DB and Plex: {manga.Name}\n";

                try
                {
                    var messageNotify = new NotifyDTO
                    {
                        Message = message
                    };
                    await _publishEndpoint.Publish(messageNotify);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
                }

                return Ok(manga);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put metadata into db
        [HttpPut("/manga/redownload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadAnimeByUrlPage(List<ChapterDTO> chapters)
        {
            try
            {
                foreach (var chapter in chapters)
                {
                    chapter.StateDownload = null;
                    await _chapterService.ResetStatusDownloadChaptersByIdAsync(chapter);
                }
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
