using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IMangaRepository
    {
        //get
        Task<List<Manga>> GetMangaAllAsync();
        Task<List<Manga>> GetMangaByNameAsync(string name);
        Task<List<Manga>> GetMostMangaByNameAsync(string name);

        //insert
        Task<Manga> InsertMangaAsync(Manga manga);

        //delete
        Task<Manga> DeleteMangaAsync(Manga manga);
    }
}
