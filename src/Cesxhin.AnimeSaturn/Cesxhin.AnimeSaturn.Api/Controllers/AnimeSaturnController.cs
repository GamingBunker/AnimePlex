using Cesxhin.AnimeSaturn.Application.HtmlAgilityPack;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Domain.DTO;
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

        //anime
        [HttpGet("/anime")]
        public async Task<IActionResult> GetAnimeAll()
        {
            try
            {
                return Ok(await _animeService.GetAnimeAllAsync());
            }
            catch
            {
                return StatusCode(501);
            }
        }

        [HttpGet("/anime/name/{name}")]
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
                return StatusCode(501);
            }
        }

        [HttpGet("/anime/names/{name}")]
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
                return StatusCode(501);
            }
        }

        [HttpPost("/anime")]
        public async Task<IActionResult> PutAnime(AnimeDTO anime)
        {
            //insert
            var animeResult = await _animeService.InsertAnimeAsync(anime);

            if (animeResult == null)
                return Conflict();

            return Created("none", animeResult);
        }

        //episode
        [HttpGet("/episode/name/{name}")]
        public async Task<IActionResult> GetEpisodeByName(string name)
        {
            try
            {
                var listEpisodes = await _episodeService.GetEpisodesByNameAsync(name);
                return Ok(listEpisodes);
            }
            catch
            {
                return StatusCode(501);
            }
        }

        [HttpGet("/episode/id/{id}")]
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
                return StatusCode(501);
            }
        }

        [HttpPost("/episode")]
        public async Task<IActionResult> PutEpisode(EpisodeDTO episode)
        {
            //insert
            var episodeResult = await _episodeService.InsertEpisodeAsync(episode);

            if (episodeResult == null)
                return Conflict();

            return Created("none", episodeResult);
        }

        [HttpPost("/episodes")]
        public async Task<IActionResult> PutEpisodes(List<EpisodeDTO> episodes)
        {
            //insert
            var episodeResult = await _episodeService.InsertEpisodesAsync(episodes);

            if (episodeResult == null)
                return Conflict();

            return Created("none", episodeResult);
        }

        [HttpPut("/statusDownload")]
        public async void PutUpdateStateDownload(EpisodeDTO episode)
        {
            //update
            await _episodeService.UpdateStateDownloadAsync(episode);
        }

        [HttpGet("/animesaturn/name/{name}")]
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
                return StatusCode(501);
            }
        }

        [HttpPost("/animesaturn/download")]
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
                return StatusCode(501);
            }
        }
    }
}
