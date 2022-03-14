using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using ExperienceTrackr.Models;
using Steamworks;
using Dapper;
using Rocket.Core.Logging;

namespace ExperienceTrackr.Database
{
    public class DatabaseManager
    {
        public static ExperienceTrackr Instance;
        public static ExperienceTrackrConfiguration Config => Instance.Configuration.Instance;
        private MySqlConnection connection;
        private string connectionString => String.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};",
            ExperienceTrackr.Instance.Configuration.Instance.DatabaseHost,
            ExperienceTrackr.Instance.Configuration.Instance.DatabasePort,
            ExperienceTrackr.Instance.Configuration.Instance.DatabaseName,
            ExperienceTrackr.Instance.Configuration.Instance.DatabaseUsername,
            ExperienceTrackr.Instance.Configuration.Instance.DatabasePassword);
        private List<UserExperience> UserExperience;

        internal DatabaseManager()
        {
            // Makes new connection
            connection = new MySqlConnection(connectionString);
            connection.Open();
            CreateTablesIfNotExists();
        }
        private void CreateTablesIfNotExists()
        {
            // Creating the Tabel
            const string sql1 = "CREATE TABLE IF NOT EXISTS ExperienceTrackr (SteamId BIGINT NOT NULL, Name VARCHAR(255) NOT NULL, Experience INT NOT NULL DEFAULT 0, LastUpdated VARCHAR(255) NOT NULL);";
            connection.Execute(sql1);
        }
        public void ReadData()
        {
            const string sql1 = "SELECT * FROM `UserExperience`;";
            UserExperience = connection.Query<UserExperience>(sql1).ToList();
            if (UserExperience == null) { UserExperience = new List<UserExperience>(); }
        }
        public bool UpdatePlayerDatabase(string steamid, string name, int experience, string lastupdated)
        {
            int result = 0;
            try
            {
                using (connection)
                {
                    // If Statement Checks if they already exists in the Tabel
                    if (connection.ExecuteScalar<bool>("SELECT COUNT(1) FROM ExperienceTrackr WHERE SteamId = @SteamId;", new { SteamId = steamid }) == false)
                    {
                        // Add them to the Tabel if not already exists
                        result = connection.Execute("INSERT INTO ExperienceTrackr (SteamId, Name, Experience, LastUpdated) VALUES (@SteamId, @Name, @Experience, @LastUpdated);", new { SteamId = steamid, Name = name, Experience = experience, LastUpdated = lastupdated });
                    }
                    else if(connection.ExecuteScalar<bool>("SELECT COUNT(1) FROM ExperienceTrackr WHERE SteamId = @SteamId;", new { SteamId = steamid }) == true)
                    {
                        result = connection.Execute("UPDATE ExperienceTrackr SET Experience = @Experience, LastUpdated = @LastUpdated WHERE SteamId = @SteamId;", new { Experience = experience, SteamId = steamid, LastUpdated = lastupdated });
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            return result == 0 ? false : true;
        }
    }
}
// Make Public UpdatePlayer() to Update the Experience if they exists in the table when they join the server
