using Cesxhin.AnimeSaturn.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Services
{
    public interface IMangaService
    {
        //get
        Task<IEnumerable<GenericMangaDTO>> GetMangaAllWithAllAsync();
        Task<IEnumerable<MangaDTO>> GetMostMangaByNameAsync(string name);
        Task<IEnumerable<MangaDTO>> GetMangaAllAsync();
        Task<MangaDTO> GetMangaByNameAsync(string name);

        //insert
        Task<MangaDTO> InsertMangaAsync(MangaDTO manga);

        //delete
        Task<string> DeleteMangaByNameAsync(string name);
    }
}
