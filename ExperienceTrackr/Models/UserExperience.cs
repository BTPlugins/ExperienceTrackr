using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperienceTrackr.Models
{
    public  class UserExperience
    {
        public ulong SteamId { get; set; }
        public string Name { get; set; }
        public uint Experience { get; set; }
        public string LastUpdated { get; set; }
    }
}
