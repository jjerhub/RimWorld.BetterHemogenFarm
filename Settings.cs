using Verse;

namespace jjerhub.BetterHemogenFarmForked
{
    public class BetterHemoFarmForkedSettings : ModSettings
    {
        public bool enableFarmHemogen = true;
        public bool enableFarmOvum = false;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableFarmHemogen, "enableFarmHemogen", true);
            Scribe_Values.Look(ref enableFarmOvum, "enableFarmOvum", false);

            base.ExposeData();
        }
    }
}