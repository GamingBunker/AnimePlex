using Cesxhin.AnimeSaturn.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Services
{
    public interface IAnimeService
    {
        //get
        Task<IEnumerable<AnimeDTO>> GetAnimeAllAsync();
        Task<AnimeDTO> GetAnimeByNameAsync(string name);
        Task<IEnumerable<AnimeDTO>> GetMostAnimeByNameAsync(string name);
        Task<IEnumerable<GenericDTO>> GetAnimeAllWithAllAsync();

        //insert
        Task<AnimeDTO> InsertAnimeAsync(AnimeDTO anime);
    }
}
