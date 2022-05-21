using Cesxhin.AnimeSaturn.Domain.DTO;
using RepoDb.Attributes;
using System;

namespace Cesxhin.AnimeSaturn.Domain.Models
{
    [Map("chapterregister")]
    public class ChapterRegister
    {
        [Primary]
        [Map("chapterid")]
        public string ChapterId { get; set; }

        [Map("chapterpath")]
        public string[] ChapterPath { get; set; }

        [Map("chapterhash")]
        public string[] ChapterHash { get; set; }

        public static ChapterRegister ChapterRegisterDTOToChapterRegister(ChapterRegisterDTO anime)
        {
            return new ChapterRegister
            {
                ChapterId = anime.ChapterId,
                ChapterPath = anime.ChapterPath,
                ChapterHash = anime.ChapterHash
            };
        }
    }
}
