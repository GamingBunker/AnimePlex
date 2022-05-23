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
    public class GenericController : ControllerBase
    {
        //interfaces
        private readonly IAnimeService _animeService;
        private readonly IMangaService _mangaService;

        public GenericController(
            IAnimeService animeService,
            IMangaService mangaService
            )
        {
            _animeService = animeService;
            _mangaService = mangaService;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Tuple<GenericAnimeDTO, GenericMangaDTO>>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                List<object> listGeneric = new();
                var listAnime = await _animeService.GetAnimeAllAsync();
                var listManga = await _mangaService.GetMangaAllAsync();

                if(listAnime != null)
                    listGeneric.AddRange(listAnime);

                if(listManga != null)
                listGeneric.AddRange(listManga);

                if (listGeneric.Count <= 0)
                    return NotFound();

                return Ok(listGeneric);
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
                    var disk = new DiskSpaceDTO
                    {
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
                    if (intervalCheck != null)
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
