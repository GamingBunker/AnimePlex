using Cesxhin.AnimeSaturn.Application.Generic;
using Cesxhin.AnimeSaturn.Application.Interfaces.Repositories;
using Cesxhin.AnimeSaturn.Domain.Models;
using NLog;
using Npgsql;
using RepoDb;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cesxhin.AnimeSaturn.Persistence.Repositories
{
    public class EpisodeRegisterRepository : IEpisodeRegisterRepository
    {
        //log
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        //get all episodesRegisters
        public async Task<List<EpisodeRegister>> GetEpisodeRegisterByEpisodeId(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.QueryAsync<EpisodeRegister>(e => e.EpisodeId == id);
                    return ConvertGeneric<EpisodeRegister>.ConvertIEnurableToListCollection(rs);
                }
                catch (Exception e)
                {
                    logger.Error("Failed GetAnimeAllAsync, details error: " + e);
                    return null;
                }
            }
        }

        //insert
        public async Task<EpisodeRegister> InsertEpisodeRegisterAsync(EpisodeRegister episodeRegister)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.InsertAsync(episodeRegister);
                    return episodeRegister;
                }
                catch (Exception e)
                {
                    logger.Error("Failed GetAnimeAllAsync, details error: " + e);
                    return null;
                }
            }
        }

        //update
        public async Task<EpisodeRegister> UpdateEpisodeRegisterAsync(EpisodeRegister episodeRegister)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.UpdateAsync(episodeRegister, e => e.ID == episodeRegister.ID);

                    //check update
                    if(rs > 0)
                        return episodeRegister;
                    return null;
                }
                catch (Exception e)
                {
                    logger.Error("Failed GetAnimeAllAsync, details error: " + e);
                    return null;
                }
            }
        }
    }
}
