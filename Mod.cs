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
            Func<string,string> titleTemplate = (f) => $"{"BHF_Extract".Translate()} {f}";
            Func<string, string> descTemplate = (f) => "BHF_Sett_Gzm_Temp".Translate(f);//Whether or not to display the {0} extraction gizmo. The gizmo allows you to toggle the extraction bill to be placed on the pawn automatically as appropriate.
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            //listingStandard.CheckboxLabeled("Extract Hemogen", ref settings.enableFarmHemogen, "Whether or not to display the hemogen extraction gizmo. The gizmo allows you to toggle the extraction bill to be placed on the pawn automatically as appropriate.");
            //listingStandard.CheckboxLabeled("Extract Ovum", ref settings.enableFarmOvum, "Whether or not to display the ovum extraction gizmo. The gizmo allows you to toggle the extraction bill to be placed on the pawn automatically as appropriate.");
            listingStandard.CheckboxLabeled(titleTemplate(hemo), ref settings.enableFarmHemogen, descTemplate(hemo.ToLower()));
            listingStandard.CheckboxLabeled(titleTemplate(ovum), ref settings.enableFarmOvum, descTemplate(ovum.ToLower()));
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
    }
}