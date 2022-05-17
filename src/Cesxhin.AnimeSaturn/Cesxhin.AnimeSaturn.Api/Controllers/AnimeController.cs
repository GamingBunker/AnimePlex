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
    public class AnimeController : ControllerBase, IGeneralControllerBase<AnimeDTO, EpisodeDTO, EpisodeRegisterDTO, DownloadDTO>
    {
        //interfaces
        private readonly IAnimeService _animeService;
        private readonly IEpisodeService _episodeService;
        private readonly IEpisodeRegisterService _episodeRegisterService;
        private readonly IBus _publishEndpoint;

        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";

        public AnimeController(
            IAnimeService animeService,
            IEpisodeService episodeService,
            IEpisodeRegisterService episodeRegisterService,
            IBus publishEndpoint
            )
        {
            _animeService = animeService;
            _episodeService = episodeService;
            _episodeRegisterService = episodeRegisterService;
            _publishEndpoint = publishEndpoint;
        }

        //get list all anime without filter
        [HttpGet("/anime")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AnimeDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoAll()
        {
            try
            {
                var listAnime = await _animeService.GetAnimeAllAsync();

                if (listAnime == null)
                    return NotFound();

                return Ok(listAnime);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get anime by name
        [HttpGet("/anime/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimeDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInfoByName(string name)
        {
            try
            {
                var anime = await _animeService.GetAnimeByNameAsync(name);

                if (anime == null)
                    return NotFound();

                return Ok(anime);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get list anime by start name similar
        [HttpGet("/anime/names/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AnimeDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMostInfoByName(string name)
        {
            try
            {
                var anime = await _animeService.GetMostAnimeByNameAsync(name);

                if (anime == null)
                    return NotFound();

                return Ok(anime);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get list episode by name anime
        [HttpGet("/episode/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EpisodeDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectByName(string name)
        {
            try
            {
                var listEpisodes = await _episodeService.GetEpisodesByNameAsync(name);

                if (listEpisodes == null)
                    return NotFound();

                return Ok(listEpisodes);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get one episode by id
        [HttpGet("/episode/id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EpisodeDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectById(string id)
        {
            try
            {
                var episode = await _episodeService.GetEpisodeByIDAsync(id);

                if (episode == null)
                    return NotFound();

                return Ok(episode);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get one episode by id
        [HttpGet("/episode/register/episodeid/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EpisodeRegisterDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetObjectRegisterByObjectId(string id)
        {
            try
            {
                var episodeRegister = await _episodeRegisterService.GetEpisodeRegisterByEpisodeId(id);

                if (episodeRegister == null)
                    return NotFound();

                return Ok(episodeRegister);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //insert anime
        [HttpPost("/anime")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimeDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutInfo(AnimeDTO infoClass)
        {
            try
            {
                //insert
                var animeResult = await _animeService.InsertAnimeAsync(infoClass);

                if (animeResult == null)
                    return Conflict();

                return Created("none", animeResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //insert episode
        [HttpPost("/episode")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(EpisodeDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObject(EpisodeDTO objectClass)
        {
            try
            {
                //insert
                var episodeResult = await _episodeService.InsertEpisodeAsync(objectClass);

                if (episodeResult == null)
                    return Conflict();

                return Created("none", episodeResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //insert list episodes
        [HttpPost("/episodes")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<EpisodeDTO>))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObjects(List<EpisodeDTO> objectsClass)
        {
            try
            {
                //insert
                var episodeResult = await _episodeService.InsertEpisodesAsync(objectsClass);

                if (episodeResult == null)
                    return Conflict();

                return Created("none", episodeResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //insert list episodesRegisters
        [HttpPost("/episodes/registers")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(IEnumerable<EpisodeRegisterDTO>))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutObjectsRegisters(List<EpisodeRegisterDTO> objectsRegistersClass)
        {
            try
            {
                //insert
                var episodeResult = await _episodeRegisterService.InsertEpisodesRegistersAsync(objectsRegistersClass);

                if (episodeResult == null)
                    return Conflict();

                return Created("none", episodeResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put metadata into db
        [HttpPut("/episode/register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EpisodeRegisterDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateObjectRegister(EpisodeRegisterDTO objectRegisterClass)
        {
            try
            {
                var rs = await _episodeRegisterService.UpdateEpisodeRegisterAsync(objectRegisterClass);
                if (rs == null)
                    return NotFound();

                return Ok(rs);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put metadata into db
        [HttpPost("/anime/download")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimeDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadInfoByUrlPage(DownloadDTO downloadClass)
        {
            try
            {
                //get anime and episodes
                var anime = HtmlAnimeSaturn.GetAnime(downloadClass.Url);
                var episodes = HtmlAnimeSaturn.GetEpisodes(downloadClass.Url, anime.Name);

                //insert anime
                var animeResult = await _animeService.InsertAnimeAsync(anime);

                if (animeResult == null)
                    return Conflict();

                //insert episodes
                var episodeResult = await _episodeService.InsertEpisodesAsync(episodes);

                if (episodeResult == null)
                    return Conflict();

                var listEpisodeRegister = new List<EpisodeRegisterDTO>();

                foreach (var episode in episodes)
                {
                    listEpisodeRegister.Add(new EpisodeRegisterDTO
                    {
                        EpisodeId = episode.ID,
                        EpisodePath = $"{_folder}/{episode.AnimeId}/Season {episode.NumberSeasonCurrent.ToString("D2")}/{episode.AnimeId} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString("D2")}.mp4"
                    });
                }

                //insert episodesRegisters
                var episodeRegisterResult = await _episodeRegisterService.InsertEpisodesRegistersAsync(listEpisodeRegister);

                if (episodeResult == null)
                    return Conflict();

                //create message for notify
                string message = $"🧮ApiService say: \nAdd new Anime: {anime.Name}\n";

                try
                {
                    var messageNotify = new NotifyDTO
                    {
                        Message = message,
                        Image = anime.Image
                    };
                    await _publishEndpoint.Publish(messageNotify);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Cannot send message rabbit, details: {ex.Message}");
                }

                return Created("none", animeResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put metadata into db
        [HttpPut("/anime/redownload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RedownloadObjectByUrlPage(List<EpisodeDTO> objectsClass)
        {
            try
            {
                foreach (var episode in objectsClass)
                {
                    episode.StateDownload = null;
                    await _episodeService.ResetStatusDownloadEpisodesByIdAsync(episode);
                }
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //delete anime
        [HttpDelete("/anime/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimeDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteInfo(string id)
        {
            try
            {
                //insert
                var animeResult = await _animeService.DeleteAnimeAsync(id);

                if (animeResult == null)
                    return NotFound();
                else if (animeResult == "-1")
                    return Conflict();

                //create message for notify
                string message = $"🧮ApiService say: \nRemoved this Anime by DB and Plex: {id}\n";

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

                return Ok(animeResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get all db
        [HttpGet("/anime/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GenericAnimeDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var listAnime = await _animeService.GetAnimeAllWithAllAsync();

                if (listAnime == null)
                    return NotFound();

                return Ok(listAnime);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get list name by external db
        [HttpGet("/anime/list/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GenericUrlDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListSearchByName(string name)
        {
            try
            {
                var animeUrls = HtmlAnimeSaturn.GetAnimeUrl(name);
                if (animeUrls != null || animeUrls.Count >= 0)
                {
                    //list anime
                    List<GenericUrlDTO> list = new();

                    foreach (var animeUrl in animeUrls)
                    {
                        var animeUrlDTO = GenericUrlDTO.GenericUrlToGenericUrlDTO(animeUrl);

                        //check if already exists
                        var anime = await _episodeService.GetEpisodesByNameAsync(animeUrlDTO.Name);
                        if (anime != null)
                            animeUrlDTO.Exists = true;

                        list.Add(animeUrlDTO);
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

        //update status of someone episode
        [HttpPut("/anime/statusDownload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EpisodeDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUpdateStateDownload(EpisodeDTO objectClass)
        {
            try
            {
                //update
                var rs = await _episodeService.UpdateStateDownloadAsync(objectClass);
                if (rs == null)
                    return NotFound();

                return Ok(rs);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
