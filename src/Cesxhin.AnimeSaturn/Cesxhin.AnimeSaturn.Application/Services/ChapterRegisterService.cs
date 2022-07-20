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

        //get chapterRegister by chapter id
        public async Task<ChapterRegisterDTO> GetObjectRegisterByObjectId(string id)
        {
            var listChapterRegisters = await _chapterRegisterRepository.GetObjectsRegisterByObjectId(id);
            foreach (var chapterRegister in listChapterRegisters)
            {
                return ChapterRegisterDTO.ChapterRegisterToChapterRegisterDTO(chapterRegister);
            }
            return null;
        }

        //insert chapterRegister
        public async Task<ChapterRegisterDTO> InsertObjectRegisterAsync(ChapterRegisterDTO chapterRegister)
        {
            var result = await _chapterRegisterRepository.InsertObjectRegisterAsync(ChapterRegister.ChapterRegisterDTOToChapterRegister(chapterRegister));
            return ChapterRegisterDTO.ChapterRegisterToChapterRegisterDTO(result);
        }

        //insert list chapterRegister
        public async Task<List<ChapterRegisterDTO>> InsertObjectsRegistersAsync(List<ChapterRegisterDTO> chapterRegister)
        {
            List<ChapterRegisterDTO> resultChapters = new();
            foreach (var chapter in chapterRegister)
            {
                var chapterResult = await _chapterRegisterRepository.InsertObjectRegisterAsync(ChapterRegister.ChapterRegisterDTOToChapterRegister(chapter));
                resultChapters.Add(ChapterRegisterDTO.ChapterRegisterToChapterRegisterDTO(chapterResult));
            }
            return resultChapters;
        }

        //Update chapterRegister
        public async Task<ChapterRegisterDTO> UpdateObjectRegisterAsync(ChapterRegisterDTO chapterRegister)
        {
            var chapterResult = await _chapterRegisterRepository.UpdateObjectRegisterAsync(ChapterRegister.ChapterRegisterDTOToChapterRegister(chapterRegister));
            if (chapterResult == null)
                return null;
            return ChapterRegisterDTO.ChapterRegisterToChapterRegisterDTO(chapterResult);
        }
    }
}
