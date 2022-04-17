using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IAnimeRepository
    {
        //get
        Task<List<Anime>> GetAnimeAllAsync();
        Task<List<Anime>> GetAnimeByNameAsync(string name);
        Task<List<Anime>> GetMostAnimeByNameAsync(string name);

        //Insert
        Task<Anime> InsertAnimeAsync(Anime anime);

        //delete
        Task<int> DeleteAnimeAsync(string id);

    }
}
