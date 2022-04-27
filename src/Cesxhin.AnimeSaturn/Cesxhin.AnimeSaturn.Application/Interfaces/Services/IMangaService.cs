using Cesxhin.AnimeSaturn.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Services
{
    public interface IMangaService
    {
        Task<MangaDTO> InsertMangaAsync(MangaDTO manga);
    }
}
