using Cesxhin.AnimeSaturn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Application.Interfaces.Repositories
{
    public interface IChapterRepository
    {
        Task<List<Chapter>> InsertChaptersAsync(List<Chapter> chapters);
    }
}
