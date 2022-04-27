using Cesxhin.AnimeSaturn.Domain.Models;
using System.Collections.Generic;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class ChapterDTO
    {
        public int ID { get; set; }
        public string NameManga { get; set; }
        public int CurrentVolume { get; set; }
        public float CurrentChapter { get; set; }
        public List<byte[]> Pages { get; set; }
        public string UrlPage { get; set; }

        public static ChapterDTO ChapterToChapterDTO(Chapter chapter)
        {
            return new ChapterDTO
            {
                ID = chapter.ID,
                CurrentChapter = chapter.CurrentChapter,
                Pages = chapter.Pages,
                UrlPage = chapter.UrlPage,
                CurrentVolume = chapter.CurrentVolume,
                NameManga = chapter.NameManga,
            };
        }
    }
}
