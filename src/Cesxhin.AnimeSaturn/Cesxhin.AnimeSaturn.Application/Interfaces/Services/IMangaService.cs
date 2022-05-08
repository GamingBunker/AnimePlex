using Cesxhin.AnimeSaturn.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Services
{
    public interface IMangaService
    {
        //get
        Task<List<GenericMangaDTO>> GetMangaAllWithAllAsync();
        Task<List<MangaDTO>> GetMangaAllAsync();
        Task<MangaDTO> GetMangaByName(string name);

        //insert
        Task<MangaDTO> InsertMangaAsync(MangaDTO manga);

        //delete
        Task<MangaDTO> DeleteMangaByNameAsync(string name);
    }
}
