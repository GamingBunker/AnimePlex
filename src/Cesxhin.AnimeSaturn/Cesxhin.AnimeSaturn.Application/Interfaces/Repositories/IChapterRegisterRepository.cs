using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IChapterRegisterRepository
    {
        //get
        Task<List<ChapterRegister>> GetChapterRegisterByChapterId(string id);

        //insert
        Task<ChapterRegister> InsertChapterRegisterAsync(ChapterRegister chapterRegister);

        //put
        Task<ChapterRegister> UpdateChapterRegisterAsync(ChapterRegister chapterRegister);
    }
}