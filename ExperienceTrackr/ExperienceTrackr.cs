using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Core.Logging;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Logger = Rocket.Core.Logging.Logger;
using System;
using UnityEngine;
using Rocket.API;
using ShimmyMySherbet.DiscordWebhooks.Embeded;
using ExperienceTrackr.Database;
using ExperienceTrackr.Models;

namespace ExperienceTrackr
{
    public class ExperienceTrackr : RocketPlugin<ExperienceTrackrConfiguration>
    {
        public static ExperienceTrackr Instance;
        public static ExperienceTrackrConfiguration Config => Instance.Configuration.Instance;
        internal DatabaseManager database;
        protected override void Load()
        {
            Instance = this;
            Logger.Log("#############################################", ConsoleColor.Yellow);
            Logger.Log("###        ExperienceTrackr Loaded        ###", ConsoleColor.Yellow);
            Logger.Log("###   Plugin Created By blazethrower320   ###", ConsoleColor.Yellow);
            Logger.Log("###            Join my Discord:           ###", ConsoleColor.Yellow);
            Logger.Log("###     https://discord.gg/YsaXwBSTSm     ###", ConsoleColor.Yellow);
            Logger.Log("#############################################", ConsoleColor.Yellow);
            //
            //
            if (Config.SuspiciousWebhook == "https://discordapp.com/api/webhooks/{webhook.id}/{webhook.api}" || Config.AutoBanWebhook == "https://discordapp.com/api/webhooks/{webhook.id}/{webhook.api}")
            {
                Logger.Log("-----------------------");
                Logger.Log("Default Webhook URL Found -> ExperienceTrackr.Configuration.Xml");
                Logger.Log("Default Webhook URL Found -> ExperienceTrackr.Configuration.Xml");
                Logger.Log("Default Webhook URL Found -> ExperienceTrackr.Configuration.Xml");
                Logger.Log("-----------------------");
            }
            U.Events.OnPlayerConnected += OnPlayerConnected;
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            database = new DatabaseManager();
        }
        protected override void Unload()
        {
            Logger.Log("ExperienceTrackr Unloaded");
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
        }


        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            database.UpdatePlayerDatabase(player.CSteamID.m_SteamID.ToString(), player.CharacterName, (int)player.Experience, DateTime.Now.ToString());
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            database.UpdatePlayerDatabase(player.CSteamID.m_SteamID.ToString(), player.CharacterName, (int)player.Experience, DateTime.Now.ToString());
            // Auto Ban Dectection
            if (!player.HasPermission(Config.ImmunityPermission) && player.Experience >= Config.AutoBanAmount && Config.AutoBanEnable == true)
            {
                Provider.ban(player.CSteamID, Config.AutoBanReason, 99999999);
                WebhookMessage AutoBanMessage = new WebhookMessage()
                .PassEmbed()
                .WithTitle("Trackr >> Auto Ban")
                .WithDescription(player.CharacterName + " has been banned due to having a Suspicious Amount of Experience!")
                .WithColor(ShimmyMySherbet.DiscordWebhooks.Embeded.EmbedColor.Red)
                .WithURL("https://steamcommunity.com/profiles/" + player.CSteamID)
                .WithTimestamp(DateTime.Now)
                .WithField("Username", player.CharacterName)
                .WithField("SteamId", player.CSteamID.ToString())
                .WithField("Flag Amount", Config.AutoBanAmount.ToString())
                .WithField("Experience", player.Experience.ToString())
                .WithField("Reason", Config.AutoBanReason)
                .Finalize();
                DiscordWebhookService.PostMessageAsync(Config.AutoBanWebhook, AutoBanMessage);
            }
            // Suspicious Dectection
            if (!player.HasPermission(Config.ImmunityPermission) && player.Experience >= Config.SuspiciousAmount && Config.SuspiciousEnable == true)
            {
                WebhookMessage SuspiciousMessage = new WebhookMessage()
                .PassEmbed()
                .WithTitle("Trackr >> Suspicious Amount")
                .WithDescription(player.CharacterName + " was flagged for having a Suspicious Amount of Experience!")
                .WithColor(ShimmyMySherbet.DiscordWebhooks.Embeded.EmbedColor.Red)
                .WithURL("https://steamcommunity.com/profiles/" + player.CSteamID)
                .WithTimestamp(DateTime.Now)
                .WithField("Username", player.CharacterName)
                .WithField("SteamId", player.CSteamID.ToString())
                .WithField("Flag Amount", Config.SuspiciousAmount.ToString())
                .WithField("Experience", player.Experience.ToString())
                .Finalize();
                DiscordWebhookService.PostMessageAsync(Config.SuspiciousWebhook, SuspiciousMessage);
                Logger.Log(player.CharacterName + " was detected for having " + player.Experience.ToString() + " Experience!");
            }
        }
        public override TranslationList DefaultTranslations => new TranslationList
        {
            {"CheckEXP_WrongUsage", "<color=#F3F3F3>Proper Usage</color> <color=#3E65FF>/CheckEXP <Target></color>"},
            {"TargetNotFound", "<color+FF0000>Target</color> <color=#F3F3F3>not found!</color>" },
            {"CheckServer_TotalSuspicious", "<color=#FF0000>[ExperienceTrackr]</color> <color=#3E65FF>{0}</color> <color=#F3F3F3>suspicious players were found! Please head to the</color> <color=#3E65FF>discord</color> <color=#F3F3F3>for more information!</color>"},
            {"CheckingServer_TotalAutoBans", "<color=#FF0000>[ExperienceTrackr]</color> <color=#3E65FF>{0}</color> <color=#F3F3F3>players were AutoBanned! Please head to the</color> <color=#3E65FF>discord</color> <color=#F3F3F3>for more information!</color>"},
            {"CheckEXP_EXPShown", "<color=#FF0000>[ExperienceTrackr]</color> <color=#3E65FF>{0}</color> <color=#F3F3F3>currently has</color> <color=#3E65FF>{1}</color> <color=#F3F3F3>Experience!</color>" },
            {"CheckEXP_Suspicious", "<color=#FF0000>[ExperienceTrackr]</color> <color=#3E65FF>{0}</color> <color=#F3F3F3>has been</color> <color=#FF0000>flagged</color> <color=#F3F3F3>for having a</color> <color=#FF0000>Suspicious</color> <color=#F3F3F3>amount of Experience! Please head to the</color> <color=#3E65FF>discord</color> <color=#F3F3F3>for more information!</color>"},
            {"CheckEXP_AutoBanned", "<color=#FF0000>[ExperienceTrackr]</color> <color=#3E65FF>{0} has been</color> <color=#FF0000>Auto-Banned</color> <color=#F3F3F3>for having a</color> <color=#FF0000>Suspicious</color> <color=#F3F3F3>amount of Experience! Please head to the</color> <color=#3E65FF>discord</color> <color=#F3F3F3>for more information!</color>"},
            {"CheckEXP_Clean", "<color=#FF0000>[ExperienceTrackr]</color> <color=#3E65FF>{0}</color> has a <color=#4CDB3D>clean</color> profile and has <color=#4CDB3D>not</color> been flagged by ExperienceTrackr!"},
        };
    }
}
/*
#F3F3F3 - White
#FF0000 - Red
#3E65FF - Blue
#4CDB3D - Green
*/
