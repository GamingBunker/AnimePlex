using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IEpisodeRepository
    {
        //get
        Task<IEnumerable<Episode>> GetEpisodeByIDAsync(string id);
        Task<IEnumerable<Episode>> GetEpisodesByNameAsync(string name);

        //insert
        Task<Episode> InsertEpisodeAsync(Episode episode);

        //update
        Task<Episode> UpdateStateDownloadAsync(Episode episode);

        //reset
        Task<Episode> ResetStatusDownloadEpisodesByIdAsync(Episode episode);
    }
}
