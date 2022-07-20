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
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Persistence.Repositories
{
    public class MangaRepository : IMangaRepository
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        public async Task<int> DeleteNameAsync(string name)
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    return await connection.DeleteAsync<Manga>(e => e.Name == name);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed DeleteMangaAsync, details error: {ex.Message}");
                    return 0;
                }
            }
        }

        public async Task<List<Manga>> GetNameAllAsync()
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var manga = await connection.QueryAllAsync<Manga>();
                    return ConvertGeneric<Manga>.ConvertIEnurableToListCollection(manga);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetMangaAllAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<List<Manga>> GetNameByNameAsync(string name)
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var manga = await connection.QueryAsync<Manga>(e => e.Name == name);

                    if (manga.Count() > 0)
                        return ConvertGeneric<Manga>.ConvertIEnurableToListCollection(manga);
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetMangaByNameAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<List<Manga>> GetMostNameByNameAsync(string name)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.ExecuteQueryAsync<Manga>("SELECT * FROM manga WHERE lower(name) like '%" + name.ToLower() + "%'");
                    return ConvertGeneric<Manga>.ConvertIEnurableToListCollection(rs);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetMostMangaByNameAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }

        public async Task<Manga> InsertNameAsync(Manga manga)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.InsertAsync(manga);
                    return manga;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed InsertMangaAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }
    }
}
