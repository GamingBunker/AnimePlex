using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IAnimeRepository
    {
        //get
        Task<IEnumerable<Anime>> GetAnimeAllAsync();
        Task<IEnumerable<Anime>> GetAnimeByNameAsync(string name);

        //get
        Task<Anime> InsertAnimeAsync(Anime anime);
    }
}
