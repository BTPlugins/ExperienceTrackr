using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExperienceTrackr.Helpers;
using ShimmyMySherbet.DiscordWebhooks.Embeded;
using SDG.Unturned;

namespace ExperienceTrackr.Commands
{
    internal class CheckEXPCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "CheckEXP";

        public string Help => "";

        public string Syntax => "CheckEXP <Target>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "ExperienceTrackr.CheckEXP" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if(command.Length < 1)
            {
                TranslationHelper.SendMessageTranslation(player.CSteamID, "ProperUsage", "/CheckEXP <Target>");
                return;
            }
            var target = UnturnedPlayer.FromName(command[0]);
            if(target == null)
            {
                TranslationHelper.SendMessageTranslation(player.CSteamID, "TargetNotFound");
                return;
            }
            TranslationHelper.SendMessageTranslation(player.CSteamID, "TargetExperience");
            if (!player.HasPermission(ExperienceTrackrPlugin.Instance.Configuration.Instance.ImmunityPermission) && player.Experience >= ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.AutoBanAmount && ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.Enabled)
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
                .WithField("Flag Amount", ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.AutoBanAmount.ToString())
                .WithField("Experience", player.Experience.ToString())
                .WithField("Reason", ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.AutoBanReason)
                .Finalize();
                DiscordWebhookService.PostMessageAsync(ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.AutoBanWebhook, AutoBanMessage);
                foreach (SteamPlayer steamPlayer in Provider.clients)
                {
                    UnturnedPlayer user = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    if (user.HasPermission(ExperienceTrackrPlugin.Instance.Configuration.Instance.StaffPermission))
                    {
                        TranslationHelper.SendMessageTranslation(user.CSteamID, "AutoBanMessage", target.CharacterName);
                    }
                }
                Provider.ban(player.CSteamID, ExperienceTrackrPlugin.Instance.Configuration.Instance.AutoBanSettings.AutoBanWebhook, 99999999);
            }
            else if (!player.HasPermission(ExperienceTrackrPlugin.Instance.Configuration.Instance.ImmunityPermission) && player.Experience >= ExperienceTrackrPlugin.Instance.Configuration.Instance.SuspiciousSettings.SuspiciousAmount && ExperienceTrackrPlugin.Instance.Configuration.Instance.SuspiciousSettings.Enabled)
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
                .WithField("Flag Amount", ExperienceTrackrPlugin.Instance.Configuration.Instance.SuspiciousSettings.SuspiciousAmount.ToString())
                .WithField("Experience", player.Experience.ToString())
                .Finalize();
                DiscordWebhookService.PostMessageAsync(ExperienceTrackrPlugin.Instance.Configuration.Instance.SuspiciousSettings.SuspiciousWebhook, SuspiciousMessage);
                foreach (SteamPlayer steamPlayer in Provider.clients)
                {
                    UnturnedPlayer user = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    if (user.HasPermission(ExperienceTrackrPlugin.Instance.Configuration.Instance.StaffPermission))
                    {
                        TranslationHelper.SendMessageTranslation(user.CSteamID, "SuspiciousMessage", target.CharacterName);
                    }
                }
            }
            else
            {
                TranslationHelper.SendMessageTranslation(player.CSteamID, "TargetExperience", target.Experience);
            }
        }
    }
}
