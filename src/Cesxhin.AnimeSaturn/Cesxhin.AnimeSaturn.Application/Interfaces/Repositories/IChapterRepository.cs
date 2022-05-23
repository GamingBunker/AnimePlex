using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IChapterRepository
    {
        //get
        Task<IEnumerable<Chapter>> GetChapterByIDAsync(string id);
        Task<IEnumerable<Chapter>> GetChaptersByNameAsync(string nameManga);

        //insert
        Task<Chapter> InsertChapterAsync(Chapter chapters);

        //update
        Task<Chapter> UpdateStateDownloadAsync(Chapter chapter);

        //reset
        Task<Chapter> ResetStatusDownloadChaptersByIdAsync(Chapter chapter);

        
    }
}
