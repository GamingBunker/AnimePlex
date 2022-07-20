using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.Interfaces.Services;
using Cesxhin.AnimeSaturn.Domain.DTO;
using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Services
{
    public class MangaService : IMangaService
    {
        //interfaces
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

        //delete manga
        public async Task<string> DeleteNameByIdAsync(string id)
        {
            var manga = await _mangaRepository.GetNameByNameAsync(id);

            if (manga.Count <= 0)
                return null;

            
            var chapters = await _chapterRepository.GetObjectsByNameAsync(id);

            foreach (var chapter in chapters)
            {
                if (!(chapter.StateDownload == "completed" || chapter.StateDownload == null))
                    return "-1";
            }

            var rs = await _mangaRepository.DeleteNameAsync(manga.First().Name);

            if (rs <= 0)
                return null;

            return id;

        }

        //get all manga
        public async Task<IEnumerable<MangaDTO>> GetNameAllAsync()
        {
            List<MangaDTO> resultManga = new();

            var list = await _mangaRepository.GetNameAllAsync();
            if (list == null)
                return null;
            
            foreach (var item in list)
            {
                resultManga.Add(MangaDTO.MangaToMangaDTO(item));
            }
            return resultManga;
        }

        //get all tables
        public async Task<IEnumerable<GenericMangaDTO>> GetNameAllWithAllAsync()
        {
            List<GenericMangaDTO> listGenericDTO = new();
            List<ChapterDTO> listChapterDTO = new();
            List<ChapterRegisterDTO> listChapterRegisterDTO = new();

            var listManga = await _mangaRepository.GetNameAllAsync();
            if (listManga == null)
                return null;

            //anime
            foreach (var manga in listManga)
            {
                var chapters = await _chapterRepository.GetObjectsByNameAsync(manga.Name);

                //episodes
                foreach (var chapter in chapters)
                {
                    var chaptersRegisters = await _chapterRegisterRepository.GetObjectsRegisterByObjectId(chapter.ID);

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

        //get manga by name
        public async Task<MangaDTO> GetNameByNameAsync(string name)
        {
            var listManga = await _mangaRepository.GetNameByNameAsync(name);

            foreach (var manga in listManga)
            {
                return MangaDTO.MangaToMangaDTO(manga);
            }

            //not found
            return null;
        }

        //get list manga
        public async Task<IEnumerable<MangaDTO>> GetMostNameByNameAsync(string name)
        {
            List<MangaDTO> mangaDTO = new();

            var listManga = await _mangaRepository.GetMostNameByNameAsync(name);

            if (listManga == null || listManga.Count <= 0)
                return null;

            foreach (var manga in listManga)
            {
                mangaDTO.Add(MangaDTO.MangaToMangaDTO(manga));
            }

            return mangaDTO;
        }

        //insert one manga
        public async Task<MangaDTO> InsertNameAsync(MangaDTO manga)
        {
            var rs = await _mangaRepository.InsertNameAsync(Manga.MangaDTOToManga(manga));

            if (rs == null)
                return null;

            return MangaDTO.MangaToMangaDTO(rs);
        }
    }
}
