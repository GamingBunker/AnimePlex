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

        public MangaService(IMangaRepository mangaRepository)
        {
            _mangaRepository = mangaRepository;
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
