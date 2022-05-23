using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.Models;
using NLog;
using Npgsql;
using System;
using System.Collections.Generic;
using RepoDb;
using System.Threading.Tasks;
using Cesxhin.AnimeSaturn.Application.Generic;

namespace Cesxhin.AnimeSaturn.Persistence.Repositories
{
    public class ChapterRegisterRepository : IChapterRegisterRepository
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        public async Task<List<ChapterRegister>> GetChapterRegisterByChapterId(string id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.QueryAsync<ChapterRegister>(e => e.ChapterId == id);
                    return ConvertGeneric<ChapterRegister>.ConvertIEnurableToListCollection(rs);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetChapterRegisterByChapterId, details error: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<ChapterRegister> InsertChapterRegisterAsync(ChapterRegister chapterRegister)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.InsertAsync(chapterRegister);
                    return chapterRegister;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed InsertChapterRegisterAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<ChapterRegister> UpdateChapterRegisterAsync(ChapterRegister chapterRegister)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.UpdateAsync(chapterRegister, e => e.ChapterId == chapterRegister.ChapterId);

                    //check update
                    if (rs > 0)
                        return chapterRegister;
                    return null;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed UpdateEpisodeRegisterAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }
    }
}
