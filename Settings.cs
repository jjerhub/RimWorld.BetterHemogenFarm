using Verse;

namespace jjerhub.BetterHemogenFarmForked
{
    public class BetterHemoFarmForkedSettings : ModSettings
    {
        public bool enableFarmHemogen;
        public bool enableFarmOvum;
        public bool enableHemogenLimit;
        public bool enableOvumLimit;
        public ushort hemogenLimit;
        public ushort ovumLimit;
        public ushort inventoryPollRate;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableFarmHemogen, "enableFarmHemogen", true);
            Scribe_Values.Look(ref enableFarmOvum, "enableFarmOvum", false);
            Scribe_Values.Look(ref enableHemogenLimit, "enableHemogenLimit", false);
            Scribe_Values.Look(ref enableOvumLimit, "enableOvumLimit", false);
            Scribe_Values.Look(ref hemogenLimit, "hemogenLimit", (ushort)100);
            Scribe_Values.Look(ref ovumLimit, "ovumLimit", (ushort)100);
            Scribe_Values.Look(ref inventoryPollRate, "inventoryPollRate", (ushort)2);

            base.ExposeData();
        }
    }
}