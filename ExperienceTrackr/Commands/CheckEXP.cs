using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using ShimmyMySherbet.DiscordWebhooks.Embeded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ExperienceTrackr.Commands
{
    public class CheckEXP : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "CheckEXP";

        public string Help => "Checks players Experience";

        public string Syntax => "CheckEXP";

        public List<string> Aliases => new List<string>() { "CXP" };

        public List<string> Permissions => new List<string>() { "ExperienceTrackr.CheckEXP" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if(command.Length < 1)
            {
                ChatManager.say(player.CSteamID, ExperienceTrackr.Instance.Translate("CheckEXP_WrongUsage"), Color.red, true);
                return;
            }
            UnturnedPlayer target = UnturnedPlayer.FromName(command[0]);
            if(target == null)
            {
                ChatManager.say(player.CSteamID, ExperienceTrackr.Instance.Translate("TargetNotFound"), Color.red, true);
                return;
            }
            ChatManager.say(player.CSteamID, ExperienceTrackr.Instance.Translate("CheckEXP_EXPShown", target.CharacterName, target.Experience.ToString()), Color.red, true);
            // AUTO BAN
            if (player.Experience >= ExperienceTrackr.Instance.Configuration.Instance.AutoBanAmount && !player.HasPermission(ExperienceTrackr.Instance.Configuration.Instance.ImmunityPermission) && ExperienceTrackr.Instance.Configuration.Instance.AutoBanEnable == true)
            {
                // Send Webhook to discord
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
                //
                ChatManager.say(player.CSteamID, ExperienceTrackr.Instance.Translate("CheckEXP_AutoBanned", target.CharacterName), Color.red, true);
            }
            // Sus Detection
            else if (player.Experience >= ExperienceTrackr.Instance.Configuration.Instance.SuspiciousAmount && !player.HasPermission(ExperienceTrackr.Instance.Configuration.Instance.ImmunityPermission))
            {
                // Send Webhook to discord
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
                //
                ChatManager.say(player.CSteamID, ExperienceTrackr.Instance.Translate("CheckEXP_Suspicious", target.CharacterName), Color.red, true);
            }
            else
            {
                ChatManager.say(player.CSteamID, ExperienceTrackr.Instance.Translate("CheckEXP_Clean", target.CharacterName), Color.red, true);
            }
        }
    }
}
