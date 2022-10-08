using ShimmyMySherbet.MySQL.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperienceTrackr.Database.Models
{
    public class ExperienceTrack
    {
        [SQLPrimaryKey]
        public ulong SteamID { get; set; }
        public uint Experience { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
