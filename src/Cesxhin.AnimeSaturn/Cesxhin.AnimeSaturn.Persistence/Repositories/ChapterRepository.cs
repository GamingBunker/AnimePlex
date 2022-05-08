using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.Models;
using NLog;
using Npgsql;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Persistence.Repositories
{
    public class ChapterRepository : IChapterRepository
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        public async Task<List<Chapter>> GetChaptersByNameManga(string nameManga)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var chapters = await connection.QueryAsync<Chapter>(e => e.NameManga == nameManga);
                    return ConvertGeneric<Chapter>.ConvertIEnurableToListCollection(chapters);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetChaptersBynameMangaAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<List<Chapter>> InsertChaptersAsync(List<Chapter> chapters)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.InsertAllAsync(chapters);
                    return chapters;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetAnimeAllAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<Chapter> ResetStatusDownloadChaptersByIdAsync(Chapter chapter)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.UpdateAsync(chapter, e => e.StateDownload != "completed" && e.PercentualDownload == chapter.PercentualDownload && e.ID == chapter.ID);
                    return chapter;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed ResetStatusDownloadChaptersByIdAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }
    }
}
