using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Services
{
    public interface IGeneralObject<TObjectDTO>
    {
        //get
        Task<TObjectDTO> GetObjectByIDAsync(string id);
        Task<IEnumerable<TObjectDTO>> GetObjectsByNameAsync(string name);

        //insert
        Task<List<TObjectDTO>> InsertObjectsAsync(List<TObjectDTO> generalObjects);
        Task<TObjectDTO> InsertObjectAsync(TObjectDTO generalObject);

        //update
        Task<TObjectDTO> UpdateStateDownloadAsync(TObjectDTO generalObject);

        //reset
        Task<TObjectDTO> ResetStatusDownloadObjectByIdAsync(TObjectDTO generalObject);
    }
}
