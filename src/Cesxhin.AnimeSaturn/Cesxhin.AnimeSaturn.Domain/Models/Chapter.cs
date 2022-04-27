using Cesxhin.AnimeSaturn.Domain.DTO;
using RepoDb.Attributes;
using System.Collections.Generic;

namespace Cesxhin.AnimeSaturn.Domain.Models
{
    [Map("chapter")]
    public class Chapter
    {
        [Identity]
        [Map("id")]
        public int ID { get; set; }

        [Map("namemanga")]
        public string NameManga { get; set; }

        [Map("currentvolume")]
        public int CurrentVolume { get; set; }

        [Map("currentchapter")]
        public float CurrentChapter { get; set; }

        [Map("pages")]
        public List<byte[]> Pages { get; set; }

        [Map("urlpage")]
        public string UrlPage { get; set; }

        public static Chapter ChapterDTOToChapter(ChapterDTO chapter)
        {
            return new Chapter
            {
                ID = chapter.ID,
                CurrentChapter = chapter.CurrentChapter,
                Pages = chapter.Pages,
                UrlPage = chapter.UrlPage,
                CurrentVolume = chapter.CurrentVolume,
                NameManga = chapter.NameManga
            };
        }
    }
}
