using System;
using Verse;

namespace jjerhub.BetterHemogenFarmForked
{
    internal class Persistence
    {
        private static BetterHemoFarmForkedSettings settings;

        public static BetterHemoFarmForkedSettings Settings
        {
            get { if (settings == default) settings = new BetterHemoFarmForkedSettings(); return settings; }
            set { if (settings == default) settings = value; else throw new Exception("BHF_Err_DupeSett".Translate()); } //new Exception("Unable to assign settings more than once."); }
        }
    }
}