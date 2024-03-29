﻿using Cesxhin.AnimeSaturn.Application.Generic;
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
        private readonly NLogConsole _logger = new(LogManager.GetCurrentClassLogger());

        //env
        readonly string _connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION");

        //get episode by id
        public async Task<IEnumerable<Episode>> GetObjectsByIDAsync(string id)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.QueryAsync<Episode>(e => e.ID == id);
                    return ConvertGeneric<Episode>.ConvertIEnurableToListCollection(rs);
                }
                catch(Exception ex)
                {
                    _logger.Error($"Failed GetEpisodeByIDAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }

        //get episodes by name
        public async Task<IEnumerable<Episode>> GetObjectsByNameAsync(string name)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var rs = await connection.QueryMultipleAsync<Episode, Anime>(e => e.AnimeId == name, e=> e.Name == name || e.Surname == name);

                    //create list ienurable to list
                    var list = ConvertGeneric<Episode>.ConvertIEnurableToListCollection(rs.Item1);

                    //order by asc
                    list.Sort(delegate (Episode p1, Episode p2){ return p1.NumberEpisodeCurrent.CompareTo(p2.NumberEpisodeCurrent); });

                    return list;
                }
                catch(Exception ex)
                {
                    _logger.Error($"Failed GetEpisodesByNameAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }

        //insert episode
        public async Task<Episode> InsertObjectAsync(Episode episode)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.InsertAsync(episode);
                    return episode;
                }
                catch(Exception ex)
                {
                    _logger.Error($"Failed InsertEpisodeAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }

        //reset StatusDownlod to null
        public async Task<Episode> ResetStatusDownloadObjectByIdAsync(Episode episode)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.UpdateAsync(episode, e=> e.StateDownload != "completed" && e.PercentualDownload == episode.PercentualDownload && e.ID == episode.ID);
                    return episode;
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed GetEpisodesByNameAsync, details error: {ex.Message}");
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
                catch(Exception ex)
                {
                    _logger.Error($"Failed UpdateStateDownloadAsync, details error: {ex.Message}");
                    return null;
                }
            }
        }
    }
}
