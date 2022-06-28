using Cesxhin.AnimeSaturn.Application.HtmlAgilityPack;
using Cesxhin.AnimeSaturn.Application.Interfaces.Controllers;
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
    public class MangaController : ControllerBase, IGeneralControllerBase<MangaDTO, ChapterDTO, ChapterRegisterDTO, DownloadDTO>
    {
        //interfaces
        private readonly IMangaService _mangaService;
        private readonly IChapterService _chapterService;
        private readonly IChapterRegisterService _chapterRegisterService;
        private readonly IBus _publishEndpoint;

        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";

        public MangaController(
            IMangaService mangaService,
            IChapterService chapterService,
            IChapterRegisterService chapterRegisterService,
            IBus publishEndpoint
            )
        {
            _publishEndpoint = publishEndpoint;
            _mangaService = mangaService;
            _chapterService = chapterService;
            _chapterRegisterService = chapterRegisterService;
        }

        //get list all manga without filter
        [HttpGet("/manga")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MangaDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoAll()
        {
            try
            {
                var listManga = await _mangaService.GetMangaAllAsync();

                if (listManga == null)
                    return NotFound();

                return Ok(listManga);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get manga by name
        [HttpGet("/manga/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MangaDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoByName(string name)
        {
            try
            {
                var anime = await _mangaService.GetMangaByNameAsync(name);

                if (anime == null)
                    return NotFound();

                return Ok(anime);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get list manga by start name similar
        [HttpGet("/manga/names/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<MangaDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMostInfoByName(string name)
        {
            try
            {
                var manga = await _mangaService.GetMostMangaByNameAsync(name);

                if (manga == null)
                    return NotFound();

                return Ok(manga);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get all db manga
        [HttpGet("/manga/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GenericMangaDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var listManga = await _mangaService.GetMangaAllWithAllAsync();

                if (listManga == null)
                    return NotFound();

                return Ok(listManga);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get chapter by name anime
        [HttpGet("/chapter/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ChapterDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectByName(string name)
        {
            try
            {
                var listChapters = await _chapterService.GetChaptersByNameAsync(name);

                if (listChapters == null)
                    return NotFound();

                return Ok(listChapters);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get chapter by id
        [HttpGet("/chapter/id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChapterDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectById(string id)
        {
            try
            {
                var chapter = await _chapterService.GetChapterByIDAsync(id);

                if (chapter == null)
                    return NotFound();

                return Ok(chapter);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get chapterRegister by id
        [HttpGet("/chapter/register/chapterid/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChapterRegisterDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectRegisterByObjectId(string id)
        {
            try
            {
                var chapterRegister = await _chapterRegisterService.GetChapterRegisterByChapterId(id);

                if (chapterRegister == null)
                    return NotFound();

                return Ok(chapterRegister);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get list name by external db
        [HttpGet("/manga/list/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GenericUrlDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListSearchByName(string name)
        {
            try
            {
                var mangaUrls = HtmlMangaMangaWorld.GetMangaUrl(name);
                if (mangaUrls != null || mangaUrls.Count >= 0)
                {
                    //list manga
                    List<GenericUrlDTO> list = new();

                    foreach (var mangaUrl in mangaUrls)
                    {
                        var mangaUrlDTO = GenericUrlDTO.GenericUrlToGenericUrlDTO(mangaUrl);

                        //check if already exists
                        var manga = await _chapterService.GetChaptersByNameAsync(mangaUrlDTO.Name);
                        if (manga != null)
                            mangaUrlDTO.Exists = true;

                        list.Add(mangaUrlDTO);
                    }
                    return Ok(list);
                }
                return NotFound();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //insert manga
        [HttpPost("/manga")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MangaDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutInfo(MangaDTO infoClass)
        {
            try
            {
                //insert
                var mangaResult = await _mangaService.InsertMangaAsync(infoClass);

                if (mangaResult == null)
                    return Conflict();

                return Created("none", mangaResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //insert chapter
        [HttpPost("/chapter")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ChapterDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObject(ChapterDTO objectClass)
        {
            try
            {
                //insert
                var chapterResult = await _chapterService.InsertChapterAsync(objectClass);

                if (chapterResult == null)
                    return Conflict();

                return Created("none", chapterResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //insert list chapters
        [HttpPost("/chapters")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<ChapterDTO>))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObjects(List<ChapterDTO> objectsClass)
        {
            try
            {
                //insert
                var chaptersResult = await _chapterService.InsertChaptersAsync(objectsClass);

                if (chaptersResult == null)
                    return Conflict();

                return Created("none", chaptersResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //insert list chaptersRegisters
        [HttpPost("/chapters/registers")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<ChapterRegisterDTO>))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObjectsRegisters(List<ChapterRegisterDTO> objectsRegistersClass)
        {
            try
            {
                //insert
                var chapterResult = await _chapterRegisterService.InsertChaptersRegistersAsync(objectsRegistersClass);

                if (chapterResult == null)
                    return Conflict();

                return Created("none", chapterResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put chapterRegister into db
        [HttpPut("/chapter/register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChapterRegisterDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateObjectRegister(ChapterRegisterDTO objectRegisterClass)
        {
            try
            {
                var chapterRegisterResult = await _chapterRegisterService.UpdateChapterRegisterAsync(objectRegisterClass);
                if (chapterRegisterResult == null)
                    return NotFound();

                return Ok(chapterRegisterResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }


        //reset state download of chapterRegister into db
        [HttpPut("/manga/redownload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RedownloadObjectByUrlPage(List<ChapterDTO> objectsClass)
        {
            try
            {
                foreach (var chapter in objectsClass)
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

        //put manga into db
        [HttpPost("/manga/download")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MangaDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadInfoByUrlPage(DownloadDTO objectsClass)
        {
            var urlPage = objectsClass.Url;

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

            if (chapters == null)
                return Conflict();

            //insert chapters
            var listChapters = await _chapterService.InsertChaptersAsync(chapters);

            if (listChapters == null)
                return Conflict();

            var listChapterRegister = new List<ChapterRegisterDTO>();
            List<string> chapterPaths = new();

            foreach (var chapter in chapters)
            {

                for(int i=0; i<=chapter.NumberMaxImage; i++)
                {
                    chapterPaths.Add($"{_folder}/{chapter.NameManga}/Volume {chapter.CurrentVolume}/Chapter {chapter.CurrentChapter}/{chapter.NameManga} s{chapter.CurrentVolume}c{chapter.CurrentChapter}n{i}.png");
                }

                listChapterRegister.Add(new ChapterRegisterDTO
                {
                    ChapterId = chapter.ID,
                    ChapterPath = chapterPaths.ToArray()
                });

                chapterPaths.Clear();
            }

            //insert episodesRegisters
            var episodeRegisterResult = await _chapterRegisterService.InsertChaptersRegistersAsync(listChapterRegister);

            if (episodeRegisterResult == null)
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

        //update status chapter
        [HttpPut("/manga/statusDownload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChapterDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUpdateStateDownload(ChapterDTO objectClass)
        {
            try
            {
                //update
                var chapterResult = await _chapterService.UpdateStateDownloadAsync(objectClass);
                if (chapterResult == null)
                    return NotFound();

                return Ok(chapterResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //delete manga
        [HttpDelete("/manga/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MangaDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInfo(string id)
        {
            try
            {
                var manga = await _mangaService.DeleteMangaByNameAsync(id);

                if (manga == null)
                    return NotFound();

                if (manga == "-1")
                    return Conflict();

                //create message for notify
                string message = $"🧮ApiService say: \nRemoved this Manga by DB: {id}\n";

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
    }
}
