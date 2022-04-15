using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.HtmlAgilityPack;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class AnimeSaturnController : ControllerBase
    {
        //interfaces
        private readonly IAnimeService _animeService;
        private readonly IEpisodeService _episodeService;
        private readonly IEpisodeRegisterService _episodeRegisterService;

        //env
        private readonly string _folder = Environment.GetEnvironmentVariable("BASE_PATH") ?? "/";

        public AnimeSaturnController(IAnimeService animeService, IEpisodeService episodeService, IEpisodeRegisterService episodeRegisterService)
        {
            _animeService = animeService;
            _episodeService = episodeService;
            _episodeRegisterService = episodeRegisterService;
        }

        //get list all anime without filter
        [HttpGet("/anime")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<AnimeDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAnimeAll()
        {
            try
            {
                var listAnime = await _animeService.GetAnimeAllAsync();

                if(listAnime == null)
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
        public async Task<IActionResult> GetAnimeByName (string name)
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
        public async Task<IActionResult> GetMostAnimeByName(string name)
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

        //insert anime
        [HttpPost("/anime")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimeDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutAnime(AnimeDTO anime)
        {
            try
            {
                //insert
                var animeResult = await _animeService.InsertAnimeAsync(anime);

                if (animeResult == null)
                    return Conflict();

                return Created("none", animeResult);
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
        public async Task<IActionResult> GetEpisodeByName(string name)
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
        public async Task<IActionResult> GetEpisodeById(string id)
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
        public async Task<IActionResult> GetEpisodeRegisterByEpisodeId(string id)
        {
            try
            {
                var episode = await _episodeRegisterService.GetEpisodeRegisterByEpisodeId(id);

                if (episode == null)
                    return NotFound();

                return Ok(episode);
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
        public async Task<IActionResult> PutEpisode(EpisodeDTO episode)
        {
            try
            {
                //insert
                var episodeResult = await _episodeService.InsertEpisodeAsync(episode);

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
        public async Task<IActionResult> PutEpisodes(List<EpisodeDTO> episodes)
        {
            try
            {
                //insert
                var episodeResult = await _episodeService.InsertEpisodesAsync(episodes);

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
        public async Task<IActionResult> PutEpisodesRegisters(List<EpisodeRegisterDTO> episodesRegisters)
        {
            try
            {
                //insert
                var episodeResult = await _episodeRegisterService.InsertEpisodesRegistersAsync(episodesRegisters);

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
        public async Task<IActionResult> UpdateEpisodeRegister(EpisodeRegisterDTO episodeRegister)
        {
            try
            {
                var rs = await _episodeRegisterService.UpdateEpisodeRegisterAsync(episodeRegister);
                if(rs == null)
                    return NotFound();

                return Ok(rs);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //update status of someone episode
        [HttpPut("/statusDownload")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EpisodeDTO))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUpdateStateDownload(EpisodeDTO episode)
        {
            try
            {
                //update
                var rs = await _episodeService.UpdateStateDownloadAsync(episode);
                if (rs == null)
                    return NotFound();

                return Ok(rs);
            }catch
            {
                return StatusCode(500);
            }
        }

        //get list name by external db
        [HttpGet("/animesaturn/name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AnimeUrlDTO>))]
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
                    List<AnimeUrlDTO> list = new();

                    foreach (var animeUrl in animeUrls)
                    {
                        var animeUrlDTO = new AnimeUrlDTO().AnimeToAnimeUrlDTO(animeUrl);

                        //check if already exists
                        var anime = await _episodeService.GetEpisodesByNameAsync(animeUrlDTO.Name);
                        if(anime != null)
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

        //put metadata into db
        [HttpPost("/animesaturn/download")]
        [ProducesResponseType(StatusCodes.Status201Created, Type=typeof(AnimeDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadAnimeByUrlPage(DownloadDTO download)
        {
            try
            {
                //get anime and episodes
                var anime = HtmlAnimeSaturn.GetAnime(download.Url);
                var episodes = HtmlAnimeSaturn.GetEpisodes(download.Url, anime.Name);

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
                    listEpisodeRegister.Add(new EpisodeRegisterDTO{
                        EpisodeId = episode.ID,
                        EpisodePath = $"{_folder}/{episode.AnimeId}/Season {episode.NumberSeasonCurrent.ToString("D2")}/{episode.AnimeId} s{episode.NumberSeasonCurrent.ToString("D2")}e{episode.NumberEpisodeCurrent.ToString("D2")}.mp4"
                    });
                }

                //insert episodesRegisters
                var episodeRegisterResult = await _episodeRegisterService.InsertEpisodesRegistersAsync(listEpisodeRegister);

                if (episodeResult == null)
                    return Conflict();

                return Created("none", animeResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put metadata into db
        [HttpPut("/animesaturn/redownload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadAnimeByUrlPage(List<EpisodeDTO> episodes)
        {
            try
            {
                foreach(var episode in episodes)
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

        //check test
        [HttpGet("/check")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Check()
        {
            try
            {
                return Ok("Ok");
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get all db
        [HttpGet("/all")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GenericDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _animeService.GetAnimeAllWithAllAsync();

                if(list == null)
                    return NotFound();

                return Ok(list);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put data check disk free space
        [HttpPut("/disk")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DiskSpaceDTO>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetCheckDiskFreeSpace(DiskSpaceDTO disk)
        {
            try
            {
                Environment.SetEnvironmentVariable("CHECK_DISK_FREE_SPACE", disk.DiskSizeFree.ToString());
                Environment.SetEnvironmentVariable("CHECK_DISK_TOTAL_SPACE", disk.DiskSizeTotal.ToString());
                Environment.SetEnvironmentVariable("CHECK_DISK_INTERVAL", disk.Interval.ToString());

                var check = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
                Environment.SetEnvironmentVariable("CHECK_DISK_LAST_CHECK", check.ToString());
                return Ok(disk);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get data check disk free space
        [HttpGet("/disk")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DiskSpaceDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCheckDiskFreeSpace()
        {
            try
            {
                //get
                var checkDiskFree = Environment.GetEnvironmentVariable("CHECK_DISK_FREE_SPACE");
                var checkDiskTotal = Environment.GetEnvironmentVariable("CHECK_DISK_TOTAL_SPACE");
                var lastCheck = Environment.GetEnvironmentVariable("CHECK_DISK_LAST_CHECK");
                var interval = Environment.GetEnvironmentVariable("CHECK_DISK_INTERVAL");

                //check
                if (checkDiskTotal != null && checkDiskTotal != null)
                {
                    //return with object
                    var disk = new DiskSpaceDTO{
                       DiskSizeTotal = long.Parse(checkDiskTotal),
                       DiskSizeFree = long.Parse(checkDiskFree),
                       LastCheck = long.Parse(lastCheck),
                       Interval = int.Parse(interval)
                    };
                    return Ok(disk);
                }
                else
                    return NotFound();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put data check disk free space
        [HttpPut("/health")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetHealtService(HealthDTO disk)
        {
            try
            {
                Environment.SetEnvironmentVariable($"HEALT_SERVICE_{disk.NameService.ToUpper()}_LAST_CHECK", disk.LastCheck.ToString());
                Environment.SetEnvironmentVariable($"HEALT_SERVICE_{disk.NameService.ToUpper()}_INTERVAL", disk.Interval.ToString());
                return Ok(disk);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //get data check disk free space
        [HttpGet("/health")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DiskSpaceDTO>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHealtService()
        {
            try
            {
                //set
                List<HealthDTO> healthServiceDTOs = new();

                string[] services = new string[5] { "DOWNLOAD", "UPGRADE", "API", "UPDATE", "NOTIFY" };

                var lastCheck = "";
                var intervalCheck = "";

                //gets
                foreach (string service in services)
                {
                    var health = new HealthDTO
                    {
                        NameService = service
                    };

                    //get last check
                    lastCheck = Environment.GetEnvironmentVariable($"HEALT_SERVICE_{service}_LAST_CHECK");
                    if (lastCheck != null)
                        health.LastCheck = long.Parse(lastCheck);
                    else
                        health.LastCheck = 0;

                    //get interval
                    intervalCheck = Environment.GetEnvironmentVariable($"HEALT_SERVICE_{service}_INTERVAL");
                    if(intervalCheck != null)
                        health.Interval = int.Parse(intervalCheck);
                    else
                        health.Interval = 0;

                    healthServiceDTOs.Add(health);
                }
                return Ok(healthServiceDTOs);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
