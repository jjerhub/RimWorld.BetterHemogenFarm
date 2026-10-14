using LudeonTK;
using System;
using System.Linq;
using UnityEngine;
using Verse;

namespace jjerhub.BetterHemogenFarmForked
{
    public class BetterHemoFarmForkedMod : Mod
    {
        public static Action<char, string> LogType = (t, s) =>
        {
            var message = $"{typeof(BetterHemoFarmForkedMod).Name}:: {s}";

            switch (t)
            {
                case 'i':
                case 'I':
                    Verse.Log.Message(message);
                    break;
                case 'w':
                case 'W':
                    Verse.Log.Warning(message);
                    break;
                case 'e':
                case 'E':
                    Verse.Log.Error(message);
                    break;
            }
        };
        public static readonly Action<string> Log = s => LogType('i', s);
        private readonly BetterHemoFarmForkedSettings settings;

        public BetterHemoFarmForkedMod(ModContentPack content) : base(content)
        {
            var tmpSettings = GetSettings<BetterHemoFarmForkedSettings>();

            if (tmpSettings != null)
            {
                settings = tmpSettings;
                Persistence.Settings = tmpSettings;
            }
        }

        public override string SettingsCategory() => "Better Hemogen Farm (Forked)";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            var ovum = "BHF_Ovum".Translate();
            var hemo = "BHF_Hemo".Translate();
            var indentAmount = 5f;
            string hLimitRef = settings.hemogenLimit.ToString();
            string oLimitRef = settings.ovumLimit.ToString();
            string invCacheIntervalRef = settings.inventoryPollRate.ToString();
            Func<string,string> gizmoTitleTemplate = (f) => $"{"BHF_Extract".Translate()} {f}";
            Func<string, string> gizmoDescTemplate = (f) => "BHF_Sett_Gzm_Temp".Translate(f);//Whether or not to display the {0} extraction gizmo. The gizmo allows you to toggle the extraction bill to be placed on the pawn automatically as appropriate.
            Func<string, string> limitTitleTemplate = (f) => "BHF_Sett_Limit_Enable".Translate(f);
            Func<string, string> limitValueTitleTemplate = (f) => $"{"BHF_Sett_Limit_Val".Translate(f)} ";
            Listing_Standard listingStandard = new Listing_Standard(Text.Font);

            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled(gizmoTitleTemplate(hemo), ref settings.enableFarmHemogen, gizmoDescTemplate(hemo.ToLower()));
            listingStandard.CheckboxLabeled(gizmoTitleTemplate(ovum), ref settings.enableFarmOvum, gizmoDescTemplate(ovum.ToLower()));
            listingStandard.TextFieldNumericLabeled($"{"BHF_Sett_Inv_Cache_Int".Translate()} ", ref settings.inventoryPollRate, ref invCacheIntervalRef, min: 1, max: 10);

            listingStandard.GapLine();
            listingStandard.CheckboxLabeled(limitTitleTemplate(hemo), ref settings.enableHemogenLimit);
            listingStandard.Indent(indentAmount);
            var lineRect = listingStandard.GetRect(Text.LineHeight);
            Widgets.TextFieldNumericLabeled(lineRect.OffsetSizeX(-indentAmount), limitValueTitleTemplate(hemo), ref settings.hemogenLimit, ref hLimitRef, min: 1, max: 9999);
            listingStandard.Outdent(indentAmount);
            listingStandard.GapLine();
            listingStandard.CheckboxLabeled(limitTitleTemplate(ovum), ref settings.enableOvumLimit);
            listingStandard.Indent(indentAmount);
            lineRect = listingStandard.GetRect(Text.LineHeight);
            Widgets.TextFieldNumericLabeled(lineRect.OffsetSizeX(-indentAmount), limitValueTitleTemplate(ovum), ref settings.ovumLimit, ref oLimitRef, min: 1, max: 9999);
            listingStandard.Outdent(indentAmount);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}