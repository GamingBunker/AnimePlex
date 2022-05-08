using Cesxhin.AnimeSaturn.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Services
{
    public interface IChapterService
    {
        //insert
        Task<List<ChapterDTO>> InsertChaptersAsync(List<ChapterDTO> chapters);

        //update
        Task<ChapterDTO> ResetStatusDownloadChaptersByIdAsync(ChapterDTO chapter);
    }
}
