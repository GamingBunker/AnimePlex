using Cesxhin.AnimeSaturn.Domain.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Services
{
    public interface IChapterRegisterService
    {
        //get
        Task<ChapterRegisterDTO> GetChapterRegisterByChapterId(string id);

        //insert
        Task<ChapterRegisterDTO> InsertChapterRegisterAsync(ChapterRegisterDTO chapterRegister);
        Task<List<ChapterRegisterDTO>> InsertChaptersRegistersAsync(List<ChapterRegisterDTO> chapterRegister);

        //put
        Task<ChapterRegisterDTO> UpdateChapterRegisterAsync(ChapterRegisterDTO chapterRegister);
    }
}
