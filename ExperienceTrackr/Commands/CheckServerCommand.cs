using ExperienceTrackr.Helpers;
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
            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                UnturnedPlayer user = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                // Auto Ban
                if (!player.HasPermission(ExperienceTrackrPlugin.Instance.Configuration.Instance.ImmunityPermission) && player.Experience >= ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.AutoBanAmount && ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.Enabled)
                {
                    TotalAutoBans++;
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
                    .WithField("Flag Amount", ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.AutoBanAmount.ToString())
                    .WithField("Experience", player.Experience.ToString())
                    .WithField("Reason", ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.AutoBanReason)
                    .Finalize();
                    DiscordWebhookService.PostMessageAsync(ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.AutoBanWebhook, AutoBanMessage);
                    foreach (SteamPlayer steamPlayer2 in Provider.clients)
                    {
                        UnturnedPlayer user2 = UnturnedPlayer.FromSteamPlayer(steamPlayer2);
                        if (user.HasPermission(ExperienceTrackrPlugin.Instance.Configuration.Instance.StaffPermission))
                        {
                            TranslationHelper.SendMessageTranslation(user2.CSteamID, "AutoBanMessage", user.CharacterName);
                        }
                    }
                    Provider.ban(player.CSteamID, ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.AutoBanWebhook, 99999999);
                }
                else if (!player.HasPermission(ExperienceTrackrPlugin.Instance.Configuration.Instance.ImmunityPermission) && player.Experience >= ExperienceTrackrPlugin.Instance.Configuration.Instance.SuspiciousSettings.SuspiciousAmount && ExperienceTrackrPlugin.Instance.Configuration.Instance.SuspiciousSettings.Enabled)
                {
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
                    .WithField("Flag Amount", ExperienceTrackrPlugin.Instance.Configuration.Instance.SuspiciousSettings.SuspiciousAmount.ToString())
                    .WithField("Experience", player.Experience.ToString())
                    .Finalize();
                    DiscordWebhookService.PostMessageAsync(ExperienceTrackrPlugin.Instance.Configuration.Instance.SuspiciousSettings.SuspiciousWebhook, SuspiciousMessage);
                    foreach (SteamPlayer steamPlayer2 in Provider.clients)
                    {
                        UnturnedPlayer user2 = UnturnedPlayer.FromSteamPlayer(steamPlayer2);
                        if (user.HasPermission(ExperienceTrackrPlugin.Instance.Configuration.Instance.StaffPermission))
                        {
                            TranslationHelper.SendMessageTranslation(user.CSteamID, "SuspiciousMessage", user.CharacterName);
                        }
                    }
                }

                // At end Check if TotalSus or TotalAutoBans != 0 and then use Translations
            }
            TranslationHelper.SendMessageTranslation(player.CSteamID, "CheckServer_TotalSuspicious", TotalSuspicious);
            TranslationHelper.SendMessageTranslation(player.CSteamID, "CheckingServer_TotalAutoBans", TotalAutoBans);
        }
    }
}
