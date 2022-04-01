using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IEpisodeRegisterRepository
    {
        //get
        Task<List<EpisodeRegister>> GetEpisodeRegisterByEpisodeId(int id);
        
        //insert
        Task<EpisodeRegister> InsertEpisodeRegisterAsync(EpisodeRegister episodeRegister);

        //put
        Task<EpisodeRegister> UpdateEpisodeRegisterAsync(EpisodeRegister episodeRegister);
    }
}
