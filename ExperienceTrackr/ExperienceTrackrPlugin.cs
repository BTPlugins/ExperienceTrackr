using Rocket.API;
using Rocket.Core.Plugins;
using System;
using Logger = Rocket.Core.Logging.Logger;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmyMySherbet.MySQL.EF.Core;
using ExperienceTrackr.Database;
using ShimmyMySherbet.DiscordWebhooks.Embeded;
using ExperienceTrackr.Helpers;

namespace ExperienceTrackr
{
    public partial class ExperienceTrackrPlugin : RocketPlugin<ExperienceTrackrPluginConfiguration>
    {
        public static ExperienceTrackrPlugin Instance;
        public static ExperienceTrackrPluginConfiguration Config => Instance.Configuration.Instance;
        public DatabaseManager Database { get; private set; }
        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager(ExperienceTrackrPlugin.Instance.Configuration.Instance.DatabaseSettings);
            if (!Database.Connect(out var errorMsg))
            {
                Logger.LogError($"Failed to connect to database: {errorMsg}");
                UnloadPlugin(PluginState.Failure);
                return;
            }
            Database.CheckSchema();
            Logger.Log("#############################################", ConsoleColor.Yellow);
            Logger.Log("###         ExperienceTrackr Loaded          ###", ConsoleColor.Yellow);
            Logger.Log("###   Plugin Created By blazethrower320   ###", ConsoleColor.Yellow);
            Logger.Log("###            Join my Discord:           ###", ConsoleColor.Yellow);
            Logger.Log("###     https://discord.gg/YsaXwBSTSm     ###", ConsoleColor.Yellow);
            Logger.Log("#############################################", ConsoleColor.Yellow);
            U.Events.OnPlayerConnected += OnPlayerConnected;
            if(Config.SuspiciousSettings.SuspiciousWebhook == "https://discordapp.com/api/webhooks/{webhook.id}/{webhook.api}" || Config.AutoBanSettings.AutoBanWebhook == "https://discordapp.com/api/webhooks/{webhook.id}/{webhook.api}")
            {
                Logger.Log("-----------------------");
                Logger.Log("Default Webhook URL Found -> ExperienceTrackr.Configuration.Xml");
                Logger.Log("Default Webhook URL Found -> ExperienceTrackr.Configuration.Xml");
                Logger.Log("Default Webhook URL Found -> ExperienceTrackr.Configuration.Xml");
                Logger.Log("-----------------------");
            }
        }

        private void OnPlayerConnected(UnturnedPlayer player)
        {
            var experienceDatabase = Instance.Database.ExperienceTrack;
            ThreadHelper.RunAsynchronously(async () =>
            {
                var check = await experienceDatabase.checkExists(player.CSteamID.m_SteamID);
                if (await experienceDatabase.checkExists(player.CSteamID.m_SteamID) == false)
                    await experienceDatabase.AddPlayer(player.CSteamID.m_SteamID, player.Experience);
                else
                    await experienceDatabase.UpdatePlayer(player.CSteamID.m_SteamID, player.Experience);
                //
                //
                //
                if (!player.HasPermission(Instance.Configuration.Instance.ImmunityPermission) && player.Experience >= Config.AutoBanSettings.AutoBanAmount && Config.AutoBanSettings.Enabled)
                {
                    // Ban Player
                    WebhookMessage AutoBanMessage = new WebhookMessage()
                    .PassEmbed()
                    .WithTitle("Trackr >> Auto Ban")
                    .WithDescription(player.CharacterName + " has been banned due to having a Suspicious Amount of Experience!")
                    .WithColor(EmbedColor.Red)
                    .WithURL("https://steamcommunity.com/profiles/" + player.CSteamID)
                    .WithTimestamp(DateTime.Now)
                    .WithField("Username", player.CharacterName)
                    .WithField("SteamId", player.CSteamID.ToString())
                    .WithField("Flag Amount", Config.AutoBanSettings.AutoBanAmount.ToString())
                    .WithField("Experience", player.Experience.ToString())
                    .WithField("Reason", Config.AutoBanSettings.AutoBanReason)
                    .Finalize();
                    await DiscordWebhookService.PostMessageAsync(Config.AutoBanSettings.AutoBanWebhook, AutoBanMessage);
                    Provider.ban(player.CSteamID, Config.AutoBanSettings.AutoBanWebhook, 99999999);
                }
                else if (!player.HasPermission(Config.ImmunityPermission) && player.Experience >= Config.SuspiciousSettings.SuspiciousAmount && Config.SuspiciousSettings.Enabled)
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
                    .WithField("Flag Amount", Config.SuspiciousSettings.SuspiciousAmount.ToString())
                    .WithField("Experience", player.Experience.ToString())
                    .Finalize();
                    await DiscordWebhookService.PostMessageAsync(Config.SuspiciousSettings.SuspiciousWebhook, SuspiciousMessage);
                }
            });
        }

        protected override void Unload()
        {
            Logger.Log("ExperienceTrackr Unloaded");
            U.Events.OnPlayerConnected -= OnPlayerConnected;
            Instance = null;
        }
    }
}
