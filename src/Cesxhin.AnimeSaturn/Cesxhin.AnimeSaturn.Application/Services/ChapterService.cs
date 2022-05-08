using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Services
{
    public class ChapterService : IChapterService
    {
        private readonly IChapterRepository _chapterRepository;
        public ChapterService(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }
        public async Task<List<ChapterDTO>> InsertChaptersAsync(List<ChapterDTO> chapters)
        {
            List<Chapter> insertChapters = new();
            List<ChapterDTO> outChapters = new();

            foreach (var chapter in chapters)
            {
                insertChapters.Add(Chapter.ChapterDTOToChapter(chapter));
            }

            var rs = await _chapterRepository.InsertChaptersAsync(insertChapters);

            if(rs == null)
                return null;

            foreach(var chapter in rs)
            {
                outChapters.Add(ChapterDTO.ChapterToChapterDTO(chapter));
            }

            return outChapters;
        }

        public async Task<ChapterDTO> ResetStatusDownloadChaptersByIdAsync(ChapterDTO chapter)
        {
            var rs = await _chapterRepository.ResetStatusDownloadChaptersByIdAsync(Chapter.ChapterDTOToChapter(chapter));

            if (rs == null)
                return null;

            return ChapterDTO.ChapterToChapterDTO(rs);
        }
    }
}
