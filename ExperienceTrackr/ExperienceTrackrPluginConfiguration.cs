using Rocket.API;
using ShimmyMySherbet.MySQL.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ExperienceTrackr
{
    public class ExperienceTrackrPluginConfiguration : IRocketPluginConfiguration
    {
        public DatabaseSettings DatabaseSettings = DatabaseSettings.Default;
        public string ImmunityPermission { get; set; }
        public string StaffPermission { get; set; }
        public AutoBanSettings AutoBanSettings { get; set; }
        public SuspiciousSettings SuspiciousSettings { get; set; }
        public bool DebugMode { get; set; }
        public void LoadDefaults()
        {
            DatabaseSettings = new DatabaseSettings()
            {
                DatabaseAddress = "127.0.0.1",
                DatabaseName = "unturned",
                DatabaseUsername = "root",
                DatabasePassword = "password",
                DatabasePort = 3306,
            };
            ImmunityPermission = "ExperienceTrackr.Immunity";
            AutoBanSettings = new AutoBanSettings()
            {
                Enabled = false,
                AutoBanAmount = 10000,
                AutoBanReason = "AutoBan - EXP",
                AutoBanWebhook = "https://discordapp.com/api/webhooks/{webhook.id}/{webhook.api}",
            };
            SuspiciousSettings = new SuspiciousSettings()
            {
                Enabled = false,
                SuspiciousAmount = 1000,
                SuspiciousWebhook = "https://discordapp.com/api/webhooks/{webhook.id}/{webhook.api}",
            };
            DebugMode = false;
        }
    }
    public class AutoBanSettings
    {
        public bool Enabled { get; set; }
        public int AutoBanAmount { get; set; }
        public string AutoBanWebhook { get; set; }
        public string AutoBanReason { get; set; }
    }
    public class SuspiciousSettings
    {
        public bool Enabled { get; set; }
        public int SuspiciousAmount { get; set; }
        public string SuspiciousWebhook { get; set; }
    }
}
