using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Services
{
    public class ChapterService : IChapterService
    {
        //interfaces
        private readonly IChapterRepository _chapterRepository;

        public ChapterService(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }

        //get chapter by id
        public async Task<ChapterDTO> GetChapterByIDAsync(string id)
        {
            var listChapter = await _chapterRepository.GetChapterByIDAsync(id);
            foreach (var chapter in listChapter)
            {
                return ChapterDTO.ChapterToChapterDTO(chapter);
            }
            return null;
        }

        //get chapters by name
        public async Task<IEnumerable<ChapterDTO>> GetChaptersByNameAsync(string name)
        {
            List<ChapterDTO> chapters = new();
            var listChapter = await _chapterRepository.GetChaptersByNameAsync(name);

            if (listChapter == null)
                return null;

            foreach (var chapter in listChapter)
            {
                chapters.Add(ChapterDTO.ChapterToChapterDTO(chapter));
            }

            if (chapters.Count <= 0)
                return null;

            return chapters;
        }

        //insert one chapter
        public async Task<ChapterDTO> InsertChapterAsync(ChapterDTO chapter)
        {
            var chapterResult = await _chapterRepository.InsertChapterAsync(Chapter.ChapterDTOToChapter(chapter));

            if (chapterResult == null)
                return null;

            return ChapterDTO.ChapterToChapterDTO(chapterResult);
        }

        //insert chapters
        public async Task<List<ChapterDTO>> InsertChaptersAsync(List<ChapterDTO> chapters)
        {
            List<ChapterDTO> resultChapters = new();
            foreach (var chapter in chapters)
            {
                var chapterResult = await _chapterRepository.InsertChapterAsync(Chapter.ChapterDTOToChapter(chapter));
                resultChapters.Add(ChapterDTO.ChapterToChapterDTO(chapterResult));
            }
            return resultChapters;
        }

        //reset StatusDownload to null
        public async Task<ChapterDTO> ResetStatusDownloadChaptersByIdAsync(ChapterDTO chapter)
        {
            var rs = await _chapterRepository.ResetStatusDownloadChaptersByIdAsync(Chapter.ChapterDTOToChapter(chapter));

            if (rs == null)
                return null;

            return ChapterDTO.ChapterToChapterDTO(rs);
        }

        //update PercentualState
        public async Task<ChapterDTO> UpdateStateDownloadAsync(ChapterDTO chapter)
        {
            var chapterResult = await _chapterRepository.UpdateStateDownloadAsync(Chapter.ChapterDTOToChapter(chapter));

            if (chapterResult == null)
                return null;

            return ChapterDTO.ChapterToChapterDTO(chapterResult);
        }
    }
}
