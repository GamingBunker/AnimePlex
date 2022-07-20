using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Services
{
    public class EpisodeService : IEpisodeService
    {
        //interfaces
        private readonly IEpisodeRepository _episodeRepository;

        public EpisodeService(IEpisodeRepository episodeRepository)
        {
            _episodeRepository = episodeRepository;
        }

        //get episode by id
        public async Task<EpisodeDTO> GetObjectByIDAsync(string id)
        {
            var listEpisode = await _episodeRepository.GetObjectsByIDAsync(id);
            foreach(var episode in listEpisode)
            {
                return EpisodeDTO.EpisodeToEpisodeDTO(episode);
            }
            return null;
        }

        //get episodes by name
        public async Task<IEnumerable<EpisodeDTO>> GetObjectsByNameAsync(string name)
        {
            List<EpisodeDTO> episodes = new();
            var listEpisode = await _episodeRepository.GetObjectsByNameAsync(name);

            if (listEpisode == null)
                return null;

            foreach(var episode in listEpisode)
            {
                episodes.Add(EpisodeDTO.EpisodeToEpisodeDTO(episode));
            }

            if(episodes.Count <= 0)
                return null;

            return episodes;
        }

        //insert one episode
        public async Task<EpisodeDTO> InsertObjectAsync(EpisodeDTO episode)
        {
            var episodeResult = await _episodeRepository.InsertObjectAsync(new Episode().EpisodeDTOToEpisode(episode));

            if (episodeResult == null)
                return null;

            return EpisodeDTO.EpisodeToEpisodeDTO(episodeResult);
        }

        //insert episodes
        public async Task<List<EpisodeDTO>> InsertObjectsAsync(List<EpisodeDTO> episodes)
        {
            List<EpisodeDTO> resultEpisodes = new();
            foreach(var episode in episodes)
            {
                var episodeResult = await _episodeRepository.InsertObjectAsync(new Episode().EpisodeDTOToEpisode(episode));
                resultEpisodes.Add(EpisodeDTO.EpisodeToEpisodeDTO(episodeResult));
            }
            return resultEpisodes;
        }

        //reset StatusDownload to null
        public async Task<EpisodeDTO> ResetStatusDownloadObjectByIdAsync(EpisodeDTO episode)
        {
            var episodeResult = await _episodeRepository.ResetStatusDownloadObjectByIdAsync(new Episode().EpisodeDTOToEpisode(episode));
            return EpisodeDTO.EpisodeToEpisodeDTO(episodeResult);
        }

        //update PercentualState
        public async Task<EpisodeDTO> UpdateStateDownloadAsync(EpisodeDTO episode)
        {
            var episodeResult = await _episodeRepository.UpdateStateDownloadAsync(new Episode().EpisodeDTOToEpisode(episode));

            if (episodeResult == null)
                return null;

            return EpisodeDTO.EpisodeToEpisodeDTO(episodeResult);
        }
    }
}
