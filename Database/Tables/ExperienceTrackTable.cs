using ExperienceTrackr.Database.Models;
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
        public void UpdatePlayer(ulong steamID, uint experience)
        {
            ExecuteNonQuery("UPDATE @TABLE SET Experience = @0 WHERE SteamID = @1", experience, steamID);
        }
        public bool AddPlayer(ulong steamID, uint experience)
        {
            var newPlayer = new ExperienceTrack()
            {
                SteamID = steamID,
                Experience = experience,
                LastUpdated = DateTime.UtcNow
            };
            try
            {
                Insert(newPlayer);
            }
            catch (SqlException)
            {
                return false;
            }
            return true;
        }
    }
}
