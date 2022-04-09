using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        //get episodeREgister by episode id
        public async Task<EpisodeRegisterDTO> GetEpisodeRegisterByEpisodeId(string id)
        {
            var listEpisodesRegisters = await _episodeRegisterRepository.GetEpisodeRegisterByEpisodeId(id);
            foreach(var episodeRegister in listEpisodesRegisters)
            {
                return EpisodeRegisterDTO.EpisodeRegisterToEpisodeRegisterDTO(episodeRegister);
            }
            return null;
        }

        //insert episodeRegister
        public async Task<EpisodeRegisterDTO> InsertEpisodeRegisterAsync(EpisodeRegisterDTO episodeRegister)
        {
            var rs = await _episodeRegisterRepository.InsertEpisodeRegisterAsync(EpisodeRegister.EpisodeRegisterToEpisodeRegisterDTO(episodeRegister));
            return EpisodeRegisterDTO.EpisodeRegisterToEpisodeRegisterDTO(rs);
        }

        //insert list episodeRegister
        public async Task<List<EpisodeRegisterDTO>> InsertEpisodesRegistersAsync(List<EpisodeRegisterDTO> episodesRegistersDTO)
        {
            List<EpisodeRegisterDTO> resultEpisodes = new List<EpisodeRegisterDTO>();
            foreach (var episode in episodesRegistersDTO)
            {
                var episodeResult = await _episodeRegisterRepository.InsertEpisodeRegisterAsync(EpisodeRegister.EpisodeRegisterToEpisodeRegisterDTO(episode));
                resultEpisodes.Add(EpisodeRegisterDTO.EpisodeRegisterToEpisodeRegisterDTO(episodeResult));
            }
            return resultEpisodes;
        }

        //Update episodeRegister
        public async Task<EpisodeRegisterDTO> UpdateEpisodeRegisterAsync(EpisodeRegisterDTO episodeRegister)
        {
            var rs = await _episodeRegisterRepository.UpdateEpisodeRegisterAsync(EpisodeRegister.EpisodeRegisterToEpisodeRegisterDTO(episodeRegister));
            if (rs == null)
                return null;
            return EpisodeRegisterDTO.EpisodeRegisterToEpisodeRegisterDTO(rs);
        }
    }
}
