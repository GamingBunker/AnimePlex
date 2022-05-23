using Cesxhin.AnimeSaturn.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Services
{
    public interface IChapterService
    {
        //get
        Task<ChapterDTO> GetChapterByIDAsync(string id);
        Task<IEnumerable<ChapterDTO>> GetChaptersByNameAsync(string name);

        //insert
        Task<List<ChapterDTO>> InsertChaptersAsync(List<ChapterDTO> chapters);
        Task<ChapterDTO> InsertChapterAsync(ChapterDTO chapter);

        //update
        Task<ChapterDTO> UpdateStateDownloadAsync(ChapterDTO chapter);

        //reset
        Task<ChapterDTO> ResetStatusDownloadChaptersByIdAsync(ChapterDTO chapter);
    }
}
