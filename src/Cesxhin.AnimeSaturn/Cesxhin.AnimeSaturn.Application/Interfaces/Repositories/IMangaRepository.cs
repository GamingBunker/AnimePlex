using Cesxhin.AnimeSaturn.Domain.Models;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IMangaRepository
    {
         Task<Manga> InsertMangaAsync(Manga manga);
    }
}
