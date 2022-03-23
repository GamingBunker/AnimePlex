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
        Task<IEnumerable<Anime>> GetMostAnimeByNameAsync(string name);

        //Insert
        Task<Anime> InsertAnimeAsync(Anime anime);

    }
}
