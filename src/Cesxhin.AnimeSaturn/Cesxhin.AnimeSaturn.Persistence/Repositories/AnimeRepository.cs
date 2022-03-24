using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Domain.Models;
using Npgsql;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Persistence.Repositories
{
    public class AnimeRepository : IAnimeRepository
    {
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        //get all anime
        public async Task<IEnumerable<Anime>> GetAnimeAllAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    return await connection.QueryAllAsync<Anime>();
                }
                catch
                {
                    return null;
                }
            }
        }

        //get anime by name
        public async Task<IEnumerable<Anime>> GetAnimeByNameAsync(string name)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    return await connection.QueryAsync<Anime>(e => e.Name == name);
                }catch
                {
                    return null;
                }
            }
        }

        //get
        public async Task<IEnumerable<Anime>> GetMostAnimeByNameAsync(string name)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    return await connection.ExecuteQueryAsync<Anime>("SELECT * FROM anime WHERE lower(name) like '%" + name.ToLower() + "%'");
                }
                catch
                {
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
                }catch
                {
                    return null;
                }
            }
        }
    }
}
