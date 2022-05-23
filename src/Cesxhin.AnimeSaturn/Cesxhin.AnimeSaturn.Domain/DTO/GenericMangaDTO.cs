using System.Collections.Generic;

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
