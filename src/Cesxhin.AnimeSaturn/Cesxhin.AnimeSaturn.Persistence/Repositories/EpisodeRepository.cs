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
    public class EpisodeRepository : IEpisodeRepository
    {
        //log
        private static NLogConsole logger = new NLogConsole(LogManager.GetCurrentClassLogger());

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        //get episode by id
        public async Task<IEnumerable<Episode>> GetEpisodeByIDAsync(int id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.QueryAsync<Episode>(e => e.ID == id);
                    return ConvertGeneric<Episode>.ConvertIEnurableToListCollection(rs);
                }
                catch(Exception e)
                {
                    logger.Error("Failed GetEpisodeByIDAsync, details error: " + e);
                    return null;
                }
            }
        }

        //get episodes by name
        public async Task<IEnumerable<Episode>> GetEpisodesByNameAsync(string name)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.ExecuteQueryAsync<Episode>($"SELECT * FROM episode WHERE animeid like '{name}' ORDER BY numberepisodecurrent ASC;");
                    return ConvertGeneric<Episode>.ConvertIEnurableToListCollection(rs);
                }
                catch(Exception e)
                {
                    logger.Error("Failed GetEpisodesByNameAsync, details error: " + e);
                    return null;
                }
            }
        }

        //insert episode
        public async Task<Episode> InsertEpisodeAsync(Episode episode)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.InsertAsync(episode);
                    return episode;
                }
                catch(Exception e)
                {
                    logger.Error("Failed InsertEpisodeAsync, details error: " + e);
                    return null;
                }
            }
        }

        //reset StatusDownlod to null
        public async Task<Episode> ResetStatusDownloadEpisodesByIdAsync(Episode episode)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.UpdateAsync(episode, e=> e.StateDownload != "completed" && e.PercentualDownload == episode.PercentualDownload && e.ID == episode.ID);
                    return episode;
                }
                catch (Exception e)
                {
                    logger.Error("Failed GetEpisodesByNameAsync, details error: " + e);
                    return null;
                }
            }
        }

        //update percentualDownload
        public async Task<Episode> UpdateStateDownloadAsync(Episode episode)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.UpdateAsync(episode);
                    return episode;
                }
                catch(Exception e)
                {
                    logger.Error("Failed UpdateStateDownloadAsync, details error: " + e);
                    return null;
                }
            }
        }
    }
}
