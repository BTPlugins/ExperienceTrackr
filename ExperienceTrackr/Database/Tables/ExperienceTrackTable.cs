using ExperienceTrackr.Database.Models;
using Rocket.Core.Logging;
using ShimmyMySherbet.MySQL.EF.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperienceTrackr.Database.Tables
{
    public class ExperienceTrackTable : DatabaseTable<ExperienceTrack>
    {
        public ExperienceTrackTable(string tableName) : base(tableName)
        {
        } 
        public async Task UpdatePlayer(ulong steamID, uint experience)
        {
            await ExecuteNonQueryAsync("UPDATE @TABLE SET Experience = @0 WHERE SteamID = @1", experience, steamID);
        }
        public async Task AddPlayer(ulong steamID, uint experience)
        {
            var newPlayer = new ExperienceTrack()
            {
                SteamID = steamID,
                Experience = experience,
                LastUpdated = DateTime.UtcNow
            };
            await InsertAsync(newPlayer);
        }
        public async Task<bool> checkExists(ulong playerID)
        {
            var check = await QuerySingleAsync<int>("SELECT COUNT(*) FROM @TABLE WHERE SteamID = @0", playerID);
            if(check == 0)
            {
                return false;
            }
            return true;
        }
    }
}
