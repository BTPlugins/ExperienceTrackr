using ExperienceTrackr.Database.Tables;
using ShimmyMySherbet.MySQL.EF.Core;
using ShimmyMySherbet.MySQL.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperienceTrackr.Database
{
    public class DatabaseManager : DatabaseClient
    {
        public ExperienceTrackTable ExperienceTrack { get; } = new ExperienceTrackTable("ExperienceTrackr");
        public DatabaseManager(DatabaseSettings settings) : base(settings)
        {
        }
    }
}
