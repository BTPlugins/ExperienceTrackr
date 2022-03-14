using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmyMySherbet.DiscordWebhooks.Embeded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace ExperienceTrackr.Commands
{
    public class CheckServer : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "CheckServer";

        public string Help => "Goes through all Player Experience";

        public string Syntax => "CheckServer";

        public List<string> Aliases => new List<string>() { "CS" };

        public List<string> Permissions => new List<string>() { "ExperienceTrackr.CheckServer" };
        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            int TotalSuspicious = 0;
            int TotalAutoBans = 0;
            foreach(SteamPlayer steamPlayer in Provider.clients)
            {
                UnturnedPlayer user = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                Logger.Log(user.CharacterName);
                Logger.Log(user.CSteamID.ToString());
                Logger.Log(user.Experience.ToString());
                // Auto Ban
                if (user.Experience >= ExperienceTrackr.Instance.Configuration.Instance.AutoBanAmount && !user.HasPermission(ExperienceTrackr.Instance.Configuration.Instance.ImmunityPermission) && ExperienceTrackr.Instance.Configuration.Instance.AutoBanEnable == true)
                {
                    // Send Webhook to discord
                    TotalAutoBans++;
                    Provider.ban(player.CSteamID, ExperienceTrackr.Instance.Configuration.Instance.AutoBanReason, 99999999);
                    WebhookMessage AutoBanMessage = new WebhookMessage()
                    .PassEmbed()
                    .WithTitle("Trackr >> Auto Ban")
                    .WithDescription(player.CharacterName + " has been banned due to having a Suspicious Amount of Experience!")
                    .WithColor(ShimmyMySherbet.DiscordWebhooks.Embeded.EmbedColor.Red)
                    .WithURL("https://steamcommunity.com/profiles/" + player.CSteamID)
                    .WithTimestamp(DateTime.Now)
                    .WithField("Username", player.CharacterName)
                    .WithField("SteamId", player.CSteamID.ToString())
                    .WithField("Flag Amount", ExperienceTrackr.Instance.Configuration.Instance.AutoBanAmount.ToString())
                    .WithField("Experience", player.Experience.ToString())
                    .WithField("Reason", ExperienceTrackr.Instance.Configuration.Instance.AutoBanReason)
                    .Finalize();
                    DiscordWebhookService.PostMessageAsync(ExperienceTrackr.Instance.Configuration.Instance.AutoBanWebhook, AutoBanMessage);
                }
                // Sus Detection
                else if(user.Experience >= ExperienceTrackr.Instance.Configuration.Instance.SuspiciousAmount && !user.HasPermission(ExperienceTrackr.Instance.Configuration.Instance.ImmunityPermission))
                {
                    // Send Webhook to discord
                    TotalSuspicious++;
                    WebhookMessage SuspiciousMessage = new WebhookMessage()
                    .PassEmbed()
                    .WithTitle("Trackr >> Suspicious Amount")
                    .WithDescription(player.CharacterName + " was flagged for having a Suspicious Amount of Experience!")
                    .WithColor(ShimmyMySherbet.DiscordWebhooks.Embeded.EmbedColor.Red)
                    .WithURL("https://steamcommunity.com/profiles/" + player.CSteamID)
                    .WithTimestamp(DateTime.Now)
                    .WithField("Username", player.CharacterName)
                    .WithField("SteamId", player.CSteamID.ToString())
                    .WithField("Flag Amount", ExperienceTrackr.Instance.Configuration.Instance.SuspiciousAmount.ToString())
                    .WithField("Experience", player.Experience.ToString())
                    .Finalize();
                        DiscordWebhookService.PostMessageAsync(ExperienceTrackr.Instance.Configuration.Instance.SuspiciousWebhook, SuspiciousMessage);
                    }

                // At end Check if TotalSus or TotalAutoBans != 0 and then use Translations
            }
            ChatManager.say(player.CSteamID, ExperienceTrackr.Instance.Translate("CheckServer_TotalSuspicious", TotalSuspicious.ToString()), Color.red, true);
            ChatManager.say(player.CSteamID, ExperienceTrackr.Instance.Translate("CheckingServer_TotalAutoBans", TotalAutoBans.ToString()), Color.red, true);
        }
    }
}
