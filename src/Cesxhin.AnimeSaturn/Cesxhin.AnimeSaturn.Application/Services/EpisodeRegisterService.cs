using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Services
{
    public class EpisodeRegisterService : IEpisodeRegisterService
    {
        //interfaces
        private readonly IEpisodeRegisterRepository _episodeRegisterRepository;

        public EpisodeRegisterService(IEpisodeRegisterRepository episodeRegisterRepository)
        {
            _episodeRegisterRepository = episodeRegisterRepository;
        }

        //get episodeRegister by episode id
        public async Task<EpisodeRegisterDTO> GetObjectRegisterByObjectId(string id)
        {
            var listEpisodesRegisters = await _episodeRegisterRepository.GetObjectsRegisterByObjectId(id);
            foreach(var episodeRegister in listEpisodesRegisters)
            {
                return EpisodeRegisterDTO.EpisodeRegisterToEpisodeRegisterDTO(episodeRegister);
            }
            return null;
        }

        //insert episodeRegister
        public async Task<EpisodeRegisterDTO> InsertObjectRegisterAsync(EpisodeRegisterDTO episodeRegister)
        {
            var rs = await _episodeRegisterRepository.InsertObjectRegisterAsync(EpisodeRegister.EpisodeRegisterToEpisodeRegisterDTO(episodeRegister));
            return EpisodeRegisterDTO.EpisodeRegisterToEpisodeRegisterDTO(rs);
        }

        //insert list episodeRegister
        public async Task<List<EpisodeRegisterDTO>> InsertObjectsRegistersAsync(List<EpisodeRegisterDTO> episodesRegistersDTO)
        {
            List<EpisodeRegisterDTO> resultEpisodes = new();
            foreach (var episode in episodesRegistersDTO)
            {
                var episodeResult = await _episodeRegisterRepository.InsertObjectRegisterAsync(EpisodeRegister.EpisodeRegisterToEpisodeRegisterDTO(episode));
                resultEpisodes.Add(EpisodeRegisterDTO.EpisodeRegisterToEpisodeRegisterDTO(episodeResult));
            }
            return resultEpisodes;
        }

        //Update episodeRegister
        public async Task<EpisodeRegisterDTO> UpdateObjectRegisterAsync(EpisodeRegisterDTO episodeRegister)
        {
            var rs = await _episodeRegisterRepository.UpdateObjectRegisterAsync(EpisodeRegister.EpisodeRegisterToEpisodeRegisterDTO(episodeRegister));
            if (rs == null)
                return null;
            return EpisodeRegisterDTO.EpisodeRegisterToEpisodeRegisterDTO(rs);
        }
    }
}
