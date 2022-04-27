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
    }
}
