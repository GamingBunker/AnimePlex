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
    public class MangaService : IMangaService
    {
        private readonly IMangaRepository _mangaRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IChapterRegisterRepository _chapterRegisterRepository;

        public MangaService(
            IMangaRepository mangaRepository, 
            IChapterRepository chapterRepository,
            IChapterRegisterRepository chapterRegisterRepository
            )
        {
            _mangaRepository = mangaRepository;
            _chapterRepository = chapterRepository;
            _chapterRegisterRepository = chapterRegisterRepository;
        }

        public async Task<MangaDTO> DeleteMangaByNameAsync(string name)
        {
            var manga = await _mangaRepository.GetMangaByNameAsync(name);

            if (manga.Count <= 0)
                return null;

            
            var chapters = await _chapterRepository.GetChaptersByNameAsync(name);

            foreach (var chapter in chapters)
            {
                if (!(chapter.StateDownload == "completed" || chapter.StateDownload == null))
                    return null;
            }

            var result = await _mangaRepository.DeleteMangaAsync(manga.First());

            if (result == null)
                return null;

            return MangaDTO.MangaToMangaDTO(result);
            
        }

        public async Task<IEnumerable<MangaDTO>> GetMangaAllAsync()
        {
            List<MangaDTO> resultManga = new();

            var list = await _mangaRepository.GetMangaAllAsync();
            if (list == null)
                return null;
            
            foreach (var item in list)
            {
                resultManga.Add(MangaDTO.MangaToMangaDTO(item));
            }
            return resultManga;
        }

        public async Task<IEnumerable<GenericMangaDTO>> GetMangaAllWithAllAsync()
        {
            List<GenericMangaDTO> listGenericDTO = new();
            List<ChapterDTO> listChapterDTO = new();
            List<ChapterRegisterDTO> listChapterRegisterDTO = new();

            var listManga = await _mangaRepository.GetMangaAllAsync();
            if (listManga == null)
                return null;

            //anime
            foreach (var manga in listManga)
            {
                var chapters = await _chapterRepository.GetChaptersByNameAsync(manga.Name);

                //episodes
                foreach (var chapter in chapters)
                {
                    var chaptersRegisters = await _chapterRegisterRepository.GetChapterRegisterByChapterId(chapter.ID);

                    //get first episodeRegister
                    foreach (var chapterRegister in chaptersRegisters)
                        listChapterRegisterDTO.Add(ChapterRegisterDTO.ChapterRegisterToChapterRegisterDTO(chapterRegister));

                    listChapterDTO.Add(ChapterDTO.ChapterToChapterDTO(chapter));
                }

                listGenericDTO.Add(new GenericMangaDTO
                {
                    Manga = MangaDTO.MangaToMangaDTO(manga),
                    Chapters = listChapterDTO,
                    ChapterRegister = listChapterRegisterDTO
                });

                //reset
                listChapterDTO = new();
                listChapterRegisterDTO = new();
            }

            return listGenericDTO;
        }

        public async Task<MangaDTO> GetMangaByNameAsync(string name)
        {
            var listManga = await _mangaRepository.GetMangaByNameAsync(name);

            foreach (var manga in listManga)
            {
                return MangaDTO.MangaToMangaDTO(manga);
            }

            //not found
            return null;
        }

        public async Task<IEnumerable<MangaDTO>> GetMostMangaByNameAsync(string name)
        {
            List<MangaDTO> mangaDTO = new();

            var listManga = await _mangaRepository.GetMostMangaByNameAsync(name);

            if (listManga == null || listManga.Count <= 0)
                return null;

            foreach (var manga in listManga)
            {
                mangaDTO.Add(MangaDTO.MangaToMangaDTO(manga));
            }

            return mangaDTO;
        }

        public async Task<MangaDTO> InsertMangaAsync(MangaDTO manga)
        {
            var rs = await _mangaRepository.InsertMangaAsync(Manga.MangaDTOToManga(manga));

            if (rs == null)
                return null;

            return MangaDTO.MangaToMangaDTO(rs);
        }
    }
}
