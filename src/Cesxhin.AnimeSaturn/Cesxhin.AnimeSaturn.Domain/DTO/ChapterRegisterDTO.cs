using Cesxhin.AnimeSaturn.Domain.Models;
using System;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class ChapterRegisterDTO
    {
        public string ChapterId { get; set; }
        public string[] ChapterPath { get; set; }
        public string[] ChapterHash { get; set; }

        public static ChapterRegisterDTO ChapterRegisterToChapterRegisterDTO(ChapterRegister chapter)
        {
            return new ChapterRegisterDTO
            {
                ChapterId = chapter.ChapterId,
                ChapterPath = chapter.ChapterPath,
                ChapterHash = chapter.ChapterHash
            };
        }
    }
}
