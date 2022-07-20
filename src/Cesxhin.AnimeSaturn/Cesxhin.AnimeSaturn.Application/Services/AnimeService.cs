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

        //delete anime
        public async Task<string> DeleteNameByIdAsync(string id)
        {
            //check all finish downloaded

            //get anime
            var anime = await _animeRepository.GetNameByNameAsync(id);

            if(anime == null)
                return null;

            //get episodes
            var episodes = await _episodeRepository.GetObjectsByNameAsync(id);
            
            foreach(var episode in episodes)
            {
                if (!(episode.StateDownload == "completed" || episode.StateDownload == null))
                    return "-1";
            }

            var rs = await _animeRepository.DeleteNameAsync(id);

            if (rs <= 0)
                return null;

            return id;
        }

        //get all anime
        public async Task<IEnumerable<AnimeDTO>> GetNameAllAsync()
        {
            List<AnimeDTO> animes = new();
            var listAnime = await _animeRepository.GetNameAllAsync();

            if (listAnime == null || listAnime.Count <= 0)
                return null;

            foreach (var anime in listAnime)
            {
                animes.Add(AnimeDTO.AnimeToAnimeDTO(anime));
            }

            return animes;
        }

        //get all tables
        public async Task<IEnumerable<GenericAnimeDTO>> GetNameAllWithAllAsync()
        {
            List<GenericAnimeDTO> listGenericDTO = new();
            List<EpisodeDTO> listEpisodeDTO = new();
            List<EpisodeRegisterDTO> listEpisodeRegisterDTO = new();

            var listAnime = await _animeRepository.GetNameAllAsync();
            if (listAnime == null)
                return null;

            //anime
            foreach (var anime in listAnime)
            {
                var episodes = await _episodeRepository.GetObjectsByNameAsync(anime.Name);

                //episodes
                foreach (var episode in episodes)
                {
                    var episodesRegisters = await _episodeRegisterRepository.GetObjectsRegisterByObjectId(episode.ID);

                    //get first episodeRegister
                    foreach(var episodeRegister in episodesRegisters)
                        listEpisodeRegisterDTO.Add(EpisodeRegisterDTO.EpisodeRegisterToEpisodeRegisterDTO(episodeRegister));

                    listEpisodeDTO.Add(EpisodeDTO.EpisodeToEpisodeDTO(episode));
                }

                listGenericDTO.Add(new GenericAnimeDTO
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
        public async Task<AnimeDTO> GetNameByNameAsync(string name)
        {
            var listAnime = await _animeRepository.GetNameByNameAsync(name);
            foreach(var anime in listAnime)
            {
                return AnimeDTO.AnimeToAnimeDTO(anime);
            }

            //not found
            return null;
        }

        //get list anime
        public async Task<IEnumerable<AnimeDTO>> GetMostNameByNameAsync(string name)
        {
            List<AnimeDTO> animeDTO = new();

            var listAnime = await _animeRepository.GetMostNameByNameAsync(name);

            if (listAnime == null || listAnime.Count <= 0)
                return null;

            foreach (var anime in listAnime)
            {
                animeDTO.Add(AnimeDTO.AnimeToAnimeDTO(anime));
            }

            return animeDTO;
        }

        //insert one anime
        public async Task<AnimeDTO> InsertNameAsync(AnimeDTO anime)
        {
            var animeResult = await _animeRepository.InsertNameAsync(Anime.AnimeDTOToAnime(anime));

            //get error
            if (animeResult == null)
                return null;

            return AnimeDTO.AnimeToAnimeDTO(animeResult);
        }
    }
}
