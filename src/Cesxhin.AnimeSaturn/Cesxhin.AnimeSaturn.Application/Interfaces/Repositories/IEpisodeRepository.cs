using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IEpisodeRepository
    {
        //get
        Task<IEnumerable<Episode>> GetEpisodeByIDAsync(int id);
        Task<IEnumerable<Episode>> GetEpisodesByNameAsync(string name);

        //insert
        Task<Episode> InsertEpisodeAsync(Episode episode);
    }
}
