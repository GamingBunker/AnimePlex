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

        public MangaService(IMangaRepository mangaRepository, IChapterRepository chapterRepository)
        {
            _mangaRepository = mangaRepository;
            _chapterRepository = chapterRepository;
        }

        public async Task<MangaDTO> DeleteMangaByNameAsync(string name)
        {
            var manga = await _mangaRepository.GetMangaByNameAsync(name);

            if (manga.Count <= 0)
                return null;

            var result = await _mangaRepository.DeleteMangaAsync(manga.First());

            if (result == null)
                return null;

            return MangaDTO.MangaToMangaDTO(manga.First());
            
        }

        public async Task<List<MangaDTO>> GetMangaAllAsync()
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

        public async Task<List<GenericMangaDTO>> GetMangaAllWithAllAsync()
        {
            List<GenericMangaDTO> list = new();
            var listManga = await _mangaRepository.GetMangaAllAsync();
            foreach(var manga in listManga)
            {
                var listChapters = await _chapterRepository.GetChaptersByNameManga(manga.Name);
                list.Add(new GenericMangaDTO
                {
                    Manga = manga,
                    Chapters = listChapters
                });
            }

            return list;
        }

        public async Task<MangaDTO> GetMangaByName(string name)
        {
            var manga = await _mangaRepository.GetMangaByNameAsync(name);
            if (manga == null)
                return null;

            return MangaDTO.MangaToMangaDTO(manga.First());
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
