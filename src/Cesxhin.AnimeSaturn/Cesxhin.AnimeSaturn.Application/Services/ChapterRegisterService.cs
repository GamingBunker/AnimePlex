using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Services
{
    public class ChapterRegisterService : IChapterRegisterService
    {
        //interfaces
        private readonly IChapterRegisterRepository _chapterRegisterRepository;

        public ChapterRegisterService(IChapterRegisterRepository chapterRegisterRepository)
        {
            _chapterRegisterRepository = chapterRegisterRepository;
        }

        public async Task<ChapterRegisterDTO> GetChapterRegisterByChapterId(string id)
        {
            var listChapterRegisters = await _chapterRegisterRepository.GetChapterRegisterByChapterId(id);
            foreach (var chapterRegister in listChapterRegisters)
            {
                return ChapterRegisterDTO.ChapterRegisterToChapterRegisterDTO(chapterRegister);
            }
            return null;
        }

        public async Task<ChapterRegisterDTO> InsertChapterRegisterAsync(ChapterRegisterDTO chapterRegister)
        {
            var result = await _chapterRegisterRepository.InsertChapterRegisterAsync(ChapterRegister.ChapterRegisterDTOToChapterRegister(chapterRegister));
            return ChapterRegisterDTO.ChapterRegisterToChapterRegisterDTO(result);
        }

        public async Task<List<ChapterRegisterDTO>> InsertChaptersRegistersAsync(List<ChapterRegisterDTO> chapterRegister)
        {
            List<ChapterRegisterDTO> resultChapters = new();
            foreach (var chapter in chapterRegister)
            {
                var chapterResult = await _chapterRegisterRepository.InsertChapterRegisterAsync(ChapterRegister.ChapterRegisterDTOToChapterRegister(chapter));
                resultChapters.Add(ChapterRegisterDTO.ChapterRegisterToChapterRegisterDTO(chapterResult));
            }
            return resultChapters;
        }

        public async Task<ChapterRegisterDTO> UpdateChapterRegisterAsync(ChapterRegisterDTO chapterRegister)
        {
            var chapterResult = await _chapterRegisterRepository.UpdateChapterRegisterAsync(ChapterRegister.ChapterRegisterDTOToChapterRegister(chapterRegister));
            if (chapterResult == null)
                return null;
            return ChapterRegisterDTO.ChapterRegisterToChapterRegisterDTO(chapterResult);
        }
    }
}
