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
        private static NLogConsole logger = new NLogConsole(LogManager.GetCurrentClassLogger());

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        //get all anime
        public async Task<List<Anime>> GetAnimeAllAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.QueryAllAsync<Anime>();
                    return ConvertGeneric<Anime>.ConvertIEnurableToListCollection(rs);
                }
                catch(Exception e)
                {
                    logger.Error("Failed GetAnimeAllAsync, details error: " + e);
                    return null;
                }
            }
        }

        //get anime by name
        public async Task<List<Anime>> GetAnimeByNameAsync(string name)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.QueryAsync<Anime>(e => e.Name == name);
                    return ConvertGeneric<Anime>.ConvertIEnurableToListCollection(rs);
                }
                catch(Exception e)
                {
                    logger.Error("Failed GetAnimeByNameAsync, details error: " + e);
                    return null;
                }
            }
        }

        //get
        public async Task<List<Anime>> GetMostAnimeByNameAsync(string name)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.ExecuteQueryAsync<Anime>("SELECT * FROM anime WHERE lower(name) like '%" + name.ToLower() + "%'");
                    return ConvertGeneric<Anime>.ConvertIEnurableToListCollection(rs);
                }
                catch(Exception e)
                {
                    logger.Error("Failed GetMostAnimeByNameAsync, details error: " + e);
                    return null;
                }
            }
        }

        //insert one anime
        public async Task<Anime> InsertAnimeAsync(Anime anime)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.InsertAsync(anime);
                    return anime;
                }catch(Exception e)
                {
                    logger.Error("Failed InsertAnimeAsync, details error: " + e);
                    return null;
                }
            }
        }
    }
}
