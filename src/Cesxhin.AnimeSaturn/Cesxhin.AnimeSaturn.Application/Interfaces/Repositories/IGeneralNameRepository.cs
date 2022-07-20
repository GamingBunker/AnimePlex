using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IGeneralNameRepository<TGeneralName>
    {
        //get
        Task<List<TGeneralName>> GetNameAllAsync();
        Task<List<TGeneralName>> GetNameByNameAsync(string name);
        Task<List<TGeneralName>> GetMostNameByNameAsync(string name);

        //Insert
        Task<TGeneralName> InsertNameAsync(TGeneralName generalName);

        //delete
        Task<int> DeleteNameAsync(string id);
    }
}
