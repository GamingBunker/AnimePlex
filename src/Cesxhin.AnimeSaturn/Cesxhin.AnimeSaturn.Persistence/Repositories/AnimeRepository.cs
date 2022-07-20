using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Application.NlogManager;
using Cesxhin.AnimeSaturn.Domain.Models;
using NLog;
using Npgsql;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Persistence.Repositories
{
    public class AnimeRepository : IAnimeRepository
    {
        //log
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        public async Task<int> DeleteNameAsync(string id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    return await connection.DeleteAsync<Anime>(e => e.Name == id);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetAnimeAllAsync, details error: {ex.Message}");
                    return 0;
                }
            }
        }

        //get all anime
        public async Task<List<Anime>> GetNameAllAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.QueryAllAsync<Anime>();
                    return ConvertGeneric<Anime>.ConvertIEnurableToListCollection(rs);
                }
                catch(Exception ex)
                {
                    _logger.Error($"Failed GetAnimeAllAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }

        //get anime by name
        public async Task<List<Anime>> GetNameByNameAsync(string name)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.QueryAsync<Anime>(e => e.Name == name);
                    return ConvertGeneric<Anime>.ConvertIEnurableToListCollection(rs);
                }
                catch(Exception ex)
                {
                    _logger.Error($"Failed GetAnimeByNameAsync, details error: {ex.Message}"); ;
                    return null;
                }
            }
        }

        //get
        public async Task<List<Anime>> GetMostNameByNameAsync(string name)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.ExecuteQueryAsync<Anime>("SELECT * FROM anime WHERE lower(name) like '%" + name.ToLower() + "%'");
                    return ConvertGeneric<Anime>.ConvertIEnurableToListCollection(rs);
                }
                catch(Exception ex)
                {
                    _logger.Error($"Failed GetMostAnimeByNameAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }

        //insert one anime
        public async Task<Anime> InsertNameAsync(Anime anime)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.InsertAsync(anime);
                    return anime;
                }catch(Exception ex)
                {
                    _logger.Error($"Failed InsertAnimeAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }
    }
}
