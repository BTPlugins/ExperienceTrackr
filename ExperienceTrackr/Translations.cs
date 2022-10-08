using Rocket.API.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperienceTrackr
{
    public partial class ExperienceTrackrPlugin
    {
        public override TranslationList DefaultTranslations => new TranslationList
        {
            {
                "ProperUsage", "[color=#FF0000]{{ExperienceTrackr}} [/color] [color=#F3F3F3]Proper Usage |[/color] [color=#3E65FF]{0}[/color]"
            },
            {
                "TargetNotFound", "[color=#FF0000]{{ExperienceTrackr}} [/color][color=#F3F3F3]Target Not Found![/color]"
            },
            {
                "TargetExperience", "[color=#FF0000]{{ExperienceTrackr}} [/color][color=#3E65FF]{0}[/color] [color=#F3F3F3]Currently has[/color] [color=#3E65FF]{1}[/color] [color=#F3F3F3Experience![/color]"
            },
            {
                "AutoBanMessage", "[color=#FF0000]{{ExperienceTrackr}} [/color][color=#3E65FF]{0}[/color] [color=#F3F3F3]has been Auto-Banned for having a Suspicious amount of Experience![/color]"
            },
            {
                "SuspiciousMessage", "[color=#FF0000]{{ExperienceTrackr}} [/color][color=#3E65FF]{0}[/color] [color=#F3F3F3]has a Suspicious amount of Experience. Please head to the[/color] [color=#3E65FF]Discord[/color] [color=#F3F3F3]for more Information[/color]"
            },
            {"CheckServer_TotalSuspicious", "[color=#FF0000]{{ExperienceTrackr}} [/color] [color=#3E65FF]{0}[/color] [color=#F3F3F3]suspicious players were found! Please head to the[/color] [color=#3E65FF]discord[/color] [color=#F3F3F3]for more information![/color]"},
            {"CheckingServer_TotalAutoBans", "[color=#FF0000]{{ExperienceTrackr}} [/color] [color=#3E65FF]{0}[/color] [color=#F3F3F3]players were AutoBanned! Please head to the[/color] [color=#3E65FF]discord[/color] [color=#F3F3F3]for more information![/color]"},
        };
    }
}