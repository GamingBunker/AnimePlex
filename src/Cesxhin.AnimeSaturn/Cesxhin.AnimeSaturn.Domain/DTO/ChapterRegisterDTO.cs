using Cesxhin.AnimeSaturn.Domain.Models;

namespace Cesxhin.AnimeSaturn.Domain.DTO
{
    public class ChapterRegisterDTO
    {
        public string ChapterId { get; set; }
        public string[] ChapterPath { get; set; }
        public string[] ChapterHash { get; set; }

        //convert ChapterRegister to ChapterRegisterDTO
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
