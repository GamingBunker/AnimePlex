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
        public string ID { get; set; }

        [Map("namemanga")]
        public string NameManga { get; set; }

        [Map("currentvolume")]
        public int CurrentVolume { get; set; }

        [Map("currentchapter")]
        public float CurrentChapter { get; set; }

        [Map("numbermaximage")]
        public int NumberMaxImage { get; set; }

        [Map("urlpage")]
        public string UrlPage { get; set; }

        [Map("statedownload")]
        public string StateDownload { get; set; }

        [Map("percentualdownload")]
        public int PercentualDownload { get; set; }

        public static Chapter ChapterDTOToChapter(ChapterDTO chapter)
        {
            return new Chapter
            {
                ID = chapter.ID,
                CurrentChapter = chapter.CurrentChapter,
                UrlPage = chapter.UrlPage,
                CurrentVolume = chapter.CurrentVolume,
                NameManga = chapter.NameManga,
                StateDownload = chapter.StateDownload,
                PercentualDownload = chapter.PercentualDownload,
                NumberMaxImage = chapter.NumberMaxImage
            };
        }
    }
}
