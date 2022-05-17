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
        public MangaDTO Manga { get; set; }
        public List<ChapterDTO> Chapters { get; set; }
        public List<ChapterRegisterDTO> ChapterRegister { get; set; }
    }
}
