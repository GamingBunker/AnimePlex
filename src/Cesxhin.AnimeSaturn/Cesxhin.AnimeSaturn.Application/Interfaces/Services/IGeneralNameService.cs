using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Services
{
    public interface IGeneralNameService<TGeneralNameDTO, TGenericGeneralDTO>
    {
        //get
        Task<IEnumerable<TGeneralNameDTO>> GetNameAllAsync();
        Task<TGeneralNameDTO> GetNameByNameAsync(string name);
        Task<IEnumerable<TGeneralNameDTO>> GetMostNameByNameAsync(string name);
        Task<IEnumerable<TGenericGeneralDTO>> GetNameAllWithAllAsync();

        //insert
        Task<TGeneralNameDTO> InsertNameAsync(TGeneralNameDTO anime);

        //delete
        Task<string> DeleteNameByIdAsync(string id);
    }
}
