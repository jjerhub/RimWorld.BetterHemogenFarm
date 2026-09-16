using Verse;

namespace jjerhub.BetterHemogenFarmForked
{
    public class BetterHemoFarmForkedSettings : ModSettings
    {
        public bool enableFarmHemogen;
        public bool enableFarmOvum;
        public bool enableHemogenLimit;
        public bool enableOvumLimit;
        public int hemogenLimit; //should be ushort, but not permitted by UI
        public int ovumLimit; //should be ushort, but not permitted by UI
        public int inventoryPollRate; //should be ushort, but not permitted by UI

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableFarmHemogen, "enableFarmHemogen", true);
            Scribe_Values.Look(ref enableFarmOvum, "enableFarmOvum", false);
            Scribe_Values.Look(ref enableHemogenLimit, "enableHemogenLimit", false);
            Scribe_Values.Look(ref enableOvumLimit, "enableOvumLimit", false);
            Scribe_Values.Look(ref hemogenLimit, "hemogenLimit", 100);
            Scribe_Values.Look(ref ovumLimit, "ovumLimit", 100);
            Scribe_Values.Look(ref inventoryPollRate, "inventoryPollRate", 2);

            base.ExposeData();
        }
    }
}