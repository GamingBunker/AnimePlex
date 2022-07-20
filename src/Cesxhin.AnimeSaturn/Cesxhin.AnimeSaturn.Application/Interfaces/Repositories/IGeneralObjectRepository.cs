using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IGeneralObjectRepository<T>
    {
        //get
        Task<IEnumerable<T>> GetObjectsByIDAsync(string id);
        Task<IEnumerable<T>> GetObjectsByNameAsync(string nameGeneral);

        //insert
        Task<T> InsertObjectAsync(T objectGeneral);

        //update
        Task<T> UpdateStateDownloadAsync(T objectGeneral);

        //reset
        Task<T> ResetStatusDownloadObjectByIdAsync(T objectGeneral);
    }
}
