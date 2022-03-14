using Rocket.API;
using ShimmyMySherbet.MySQL.EF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperienceTrackr
{
    public class ExperienceTrackrConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseHost { get; set; }
        public string DatabaseUsername { get; set; }
        public string DatabasePassword { get; set; }
        public string DatabasePort { get; set; }
        public string DatabaseName { get; set; }
        public string ImmunityPermission { get; set; }
        public bool AutoBanEnable { get; set; }
        public int AutoBanAmount { get; set; }
        public string AutoBanReason { get; set; }
        public string AutoBanWebhook { get; set; }
        public bool SuspiciousEnable { get; set; }
        public int SuspiciousAmount { get; set; }
        public string SuspiciousWebhook { get; set; }
        public void LoadDefaults()
        {
            DatabaseHost = "127.0.1";
            DatabaseUsername = "Username";
            DatabasePassword = "Password";
            DatabasePort = "3306";
            DatabaseName = "Database";
            ImmunityPermission = "ExperienceTrackr.Imunity";
            AutoBanEnable = true;
            AutoBanWebhook = "https://discordapp.com/api/webhooks/{webhook.id}/{webhook.api}";
            AutoBanAmount = 5000;
            AutoBanReason = "Automatic Ban: Sus EXP";
            SuspiciousEnable = true;
            SuspiciousAmount = 1000;
            SuspiciousWebhook = "https://discordapp.com/api/webhooks/{webhook.id}/{webhook.api}";
        }
    }
}
