using Cesxhin.AnimeSaturn.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Services
{
    public interface IEpisodeService
    {
        //get
        Task<EpisodeDTO> GetEpisodeByIDAsync(int id);
        Task<IEnumerable<EpisodeDTO>> GetEpisodesByNameAsync(string name);

        //insert
        Task<EpisodeDTO> InsertEpisodeAsync(EpisodeDTO episode);
        Task<List<EpisodeDTO>> InsertEpisodesAsync(List<EpisodeDTO> episode);

        //update
        Task<EpisodeDTO> UpdateStateDownloadAsync(EpisodeDTO episode);
    }
}
