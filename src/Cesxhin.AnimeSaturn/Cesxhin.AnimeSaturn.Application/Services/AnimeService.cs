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
        //interfaces
        private readonly IAnimeRepository _animeRepository;
        private readonly IEpisodeRepository _episodeRepository;
        private readonly IEpisodeRegisterRepository _episodeRegisterRepository;

        public AnimeService(IAnimeRepository animeRepository, IEpisodeRepository episodeRepository, IEpisodeRegisterRepository episodeRegisterRepository)
        {
            _animeRepository = animeRepository;
            _episodeRepository = episodeRepository;
            _episodeRegisterRepository = episodeRegisterRepository;
        }

        //get all anime
        public async Task<IEnumerable<AnimeDTO>> GetAnimeAllAsync()
        {
            List<AnimeDTO> animes = new();
            var listAnime = await _animeRepository.GetAnimeAllAsync();

            if (listAnime == null || listAnime.Count <= 0)
                return null;

            foreach (var anime in listAnime)
            {
                animes.Add(AnimeDTO.AnimeToAnimeDTO(anime));
            }

            return animes;
        }

        //get all tables
        public async Task<IEnumerable<GenericDTO>> GetAnimeAllWithAllAsync()
        {
            List<GenericDTO> listGenericDTO = new();
            List<EpisodeDTO> listEpisodeDTO = new();
            List<EpisodeRegisterDTO> listEpisodeRegisterDTO = new();

            var listAnime = await _animeRepository.GetAnimeAllAsync();
            if (listAnime == null)
                return null;

            //anime
            foreach (var anime in listAnime)
            {
                var episodes = await _episodeRepository.GetEpisodesByNameAsync(anime.Name);

                //episodes
                foreach (var episode in episodes)
                {
                    var episodesRegisters = await _episodeRegisterRepository.GetEpisodeRegisterByEpisodeId(episode.ID);

                    //get first episodeRegister
                    foreach(var episodeRegister in episodesRegisters)
                        listEpisodeRegisterDTO.Add(EpisodeRegisterDTO.EpisodeRegisterToEpisodeRegisterDTO(episodeRegister));

                    listEpisodeDTO.Add(EpisodeDTO.EpisodeToEpisodeDTO(episode));
                }

                listGenericDTO.Add(new GenericDTO
                {
                    Anime = AnimeDTO.AnimeToAnimeDTO(anime),
                    Episodes = listEpisodeDTO,
                    EpisodeRegister = listEpisodeRegisterDTO
                });

                //reset
                listEpisodeDTO = new();
                listEpisodeRegisterDTO = new();
            }

            return listGenericDTO;
        }

        //get anime by name
        public async Task<AnimeDTO> GetAnimeByNameAsync(string name)
        {
            var listAnime = await _animeRepository.GetAnimeByNameAsync(name);
            foreach(var anime in listAnime)
            {
                return AnimeDTO.AnimeToAnimeDTO(anime);
            }

            //not found
            return null;
        }

        //get list anime
        public async Task<IEnumerable<AnimeDTO>> GetMostAnimeByNameAsync(string name)
        {
            List<AnimeDTO> animeDTO = new();

            var listAnime = await _animeRepository.GetMostAnimeByNameAsync(name);

            if (listAnime == null || listAnime.Count <= 0)
                return null;

            foreach (var anime in listAnime)
            {
                animeDTO.Add(AnimeDTO.AnimeToAnimeDTO(anime));
            }

            return animeDTO;
        }

        //insert one anime
        public async Task<AnimeDTO> InsertAnimeAsync(AnimeDTO anime)
        {
            var animeResult = await _animeRepository.InsertAnimeAsync(new Anime().AnimeDTOToAnime(anime));

            //get error
            if (animeResult == null)
                return null;

            return AnimeDTO.AnimeToAnimeDTO(animeResult);
        }
    }
}
