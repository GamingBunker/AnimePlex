using Cesxhin.AnimeSaturn.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Services
{
    public interface IEpisodeRegisterService
    {
        //get
        Task<EpisodeRegisterDTO> GetEpisodeRegisterByEpisodeId(string id);

        //insert
        Task<EpisodeRegisterDTO> InsertEpisodeRegisterAsync(EpisodeRegisterDTO episodeRegister);
        Task<List<EpisodeRegisterDTO>> InsertEpisodesRegistersAsync(List<EpisodeRegisterDTO> episodeRegister);

        //put
        Task<EpisodeRegisterDTO> UpdateEpisodeRegisterAsync(EpisodeRegisterDTO episodeRegister);
    }
}
