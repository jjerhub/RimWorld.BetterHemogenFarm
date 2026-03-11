using Verse;

namespace jjerhub.BetterHemogenFarmForked
{
    public class BetterHemoFarmForkedSettings : ModSettings
    {
        public bool enableFarmHemogen = true;
        public bool enableFarmOvum = false;
        public bool enableHemogenLimit = false;
        public bool enableOvumLimit = false;
        public int hemogenLimit = 100;
        public int ovumLimit = 100;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableFarmHemogen, "enableFarmHemogen", true);
            Scribe_Values.Look(ref enableFarmOvum, "enableFarmOvum", false);
            Scribe_Values.Look(ref enableHemogenLimit, "enableHemogenLimit", false);
            Scribe_Values.Look(ref enableOvumLimit, "enableOvumLimit", false);
            Scribe_Values.Look(ref hemogenLimit, "hemogenLimit", 100);
            Scribe_Values.Look(ref ovumLimit, "ovumLimit", 100);

            base.ExposeData();
        }
    }
}