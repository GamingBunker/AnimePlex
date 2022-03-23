using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Services
{
    public class AnimeService : IAnimeService
    {
        private readonly IAnimeRepository _animeRepository;
        public AnimeService(IAnimeRepository animeRepository)
        {
            _animeRepository = animeRepository;
        }

        //get all anime
        public async Task<IEnumerable<AnimeDTO>> GetAnimeAllAsync()
        {
            List<AnimeDTO> animes = new List<AnimeDTO>();

            var listAnime = await _animeRepository.GetAnimeAllAsync();
            foreach (var anime in listAnime)
            {
                animes.Add(new AnimeDTO().AnimeToAnimeDTO(anime));
            }
            return animes;
        }

        //get anime by name
        public async Task<AnimeDTO> GetAnimeByNameAsync(string name)
        {
            var listAnime = await _animeRepository.GetAnimeByNameAsync(name);
            foreach(var anime in listAnime)
            {
                return new AnimeDTO().AnimeToAnimeDTO(anime);
            }

            //not found
            return null;
        }

        public async Task<IEnumerable<AnimeDTO>> GetMostAnimeByNameAsync(string name)
        {
            List<AnimeDTO> animeDTO = new List<AnimeDTO>();

            var listAnime = await _animeRepository.GetMostAnimeByNameAsync(name);
            foreach (var anime in listAnime)
            {
                animeDTO.Add(new AnimeDTO().AnimeToAnimeDTO(anime));
            }

            if (animeDTO.Count > 0)
                return animeDTO;
            else
                return null;
        }

        //insert one anime
        public async Task<AnimeDTO> InsertAnimeAsync(AnimeDTO anime)
        {
            var animeResult = await _animeRepository.InsertAnimeAsync(new Anime().AnimeDTOToAnime(anime));

            //get error
            if (animeResult == null)
                return null;

            return new AnimeDTO().AnimeToAnimeDTO(animeResult);
        }
    }
}
