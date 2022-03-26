using Cesxhin.AnimeSaturn.Application.HtmlAgilityPack;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class AnimeSaturnController : ControllerBase
    {
        private readonly IAnimeService _animeService;
        private readonly IEpisodeService _episodeService;
        public AnimeSaturnController(IAnimeService animeService, IEpisodeService episodeService)
        {
            _animeService = animeService;
            _episodeService = episodeService;
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
        public async Task<IActionResult> GetEpisodeById(int id)
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

        //insert list episode
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

        //update status of someone episode
        [HttpPut("/statusDownload")]
        public async Task<IActionResult> PutUpdateStateDownload(EpisodeDTO episode)
        {
            try
            {
                //update
                return Ok(await _episodeService.UpdateStateDownloadAsync(episode));
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
                    List<AnimeUrlDTO> list = new List<AnimeUrlDTO>();
                    foreach (var animeUrl in animeUrls)
                    {
                        list.Add(new AnimeUrlDTO().AnimeToAnimeUrlDTO(animeUrl));
                    }
                    return Ok(list);
                }
                else
                    return NotFound();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //put metadata into db
        [HttpPost("/animesaturn/download")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimeUrlDTO))]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadAnimeByUrlPage(DownloadDTO download)
        {
            try
            {
                var anime = HtmlAnimeSaturn.GetAnime(download.Url);
                var episodes = HtmlAnimeSaturn.GetEpisodes(download.Url, anime.Name);

                //insert
                var animeResult = await _animeService.InsertAnimeAsync(anime);

                if (animeResult == null)
                    return Conflict();

                //insert
                var episodeResult = await _episodeService.InsertEpisodesAsync(episodes);

                if (episodeResult == null)
                    return Conflict();

                return Created("none", animeResult);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        //check test
        [HttpPost("/check")]
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
    }
}
