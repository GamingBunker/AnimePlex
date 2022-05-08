using Cesxhin.AnimeSaturn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class GenericMangaDTO
    {
        //Manga
        public Manga Manga { get; set; }
        public List<Chapter> Chapters { get; set; }
    }
}
