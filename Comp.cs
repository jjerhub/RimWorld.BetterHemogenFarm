using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using FarmCriteriaSet = 
    System.Collections.Generic.Dictionary
        <
            string, 
            (
                bool allowMales, 
                bool allowPrisoners, 
                bool allowChildren, 
                bool allowSeniors, 
                bool allowInfertile, 
                bool allowHemogenic
            )?
        >;
using MyMod = jjerhub.BetterHemogenFarmForked.BetterHemoFarmForkedMod;

namespace jjerhub.BetterHemogenFarmForked
{
    public class BetterHemoFarmForkedComp : ThingComp
    {
        private Pawn Pawn => (Pawn)this.parent;
        private static readonly Func<string, string> gzmLabel = (f) => $"{"BHF_Auto".Translate()} {"BHF_Extract".Translate()} {f}";
        private static readonly string HemoGizmoLabel = gzmLabel("BHF_Hemo".Translate());//"Auto Extract Hemogen";
        private static readonly string OvumGizmoLabel = gzmLabel("BHF_Ovum".Translate());//"Auto Extract Ovum";
        private RecipeDef xtrOvum = DefDatabase<RecipeDef>.GetNamedSilentFail("ExtractOvum");
        private RecipeDef xtrHemo = RecipeDefOf.ExtractHemogenPack;
        private bool shouldFarmHemogen = false;
        private bool shouldFarmOvum = false;
        private FarmCriteriaSet billCriteria;
        private BetterHemoFarmForkedSettings settings => Persistence.Settings;
        private const ulong pollInterval = TimeSpan.TicksPerSecond / 2;
        private ulong lastTick = (ulong)DateTime.Now.Ticks + pollInterval;

        public BetterHemoFarmForkedComp() : base()
        {
            billCriteria = new FarmCriteriaSet
                {
                    {
                        xtrHemo.defName,
                        (allowMales: true, allowPrisoners: false, allowChildren: true, allowSeniors: true, allowInfertile: true, allowHemogenic: false)
                    },
                    {
                        xtrOvum.defName,
                        (allowMales: false, allowPrisoners: true, allowChildren: false, allowSeniors: false, allowInfertile: false, allowHemogenic: true)
                    }
                };
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref shouldFarmHemogen, nameof(shouldFarmHemogen), false);
            Scribe_Values.Look(ref shouldFarmOvum, nameof(shouldFarmOvum), false);
            base.PostExposeData();
        }

        //TODO: look into caching result
        private static Func<Pawn, RecipeDef, FarmCriteriaSet, bool> IsFarmableTarget = (Pawn, bill, allowRefs) =>
        {
            var allowedRef = allowRefs[bill.defName];

            return ModsConfig.BiotechActive
            && Pawn.IsFreeColonist //make sure they aren't carried by another pawn on the map
            && (!Pawn.IsPrisonerOfColony || (allowedRef?.allowPrisoners ?? false))
            && ((Pawn.Faction == Faction.OfPlayer && Pawn.HomeFaction == null) || (Pawn.HomeFaction == Faction.OfPlayer || Pawn.IsSlaveOfColony || Pawn.IsPrisonerOfColony))
            && !Pawn.IsInfant()
            && (Pawn.IsTeenOrOlder() || (allowedRef?.allowChildren ?? false))
            && (!Pawn.IsSenior() || (allowedRef?.allowSeniors ?? false))
            && (Pawn.gender == Gender.Female || (allowedRef?.allowMales ?? false))
            && ((!Pawn.Sterile() && !Pawn.SterileGenes()) || (allowedRef?.allowInfertile ?? false))
            && (!Pawn.genes.HasActiveGene(GeneDefOf.Hemogenic) || (allowedRef?.allowHemogenic ?? false));
        };

        private static Func<Pawn, RecipeDef, FarmCriteriaSet, bool> IsFarmableNow = (Pawn, bill, allowRef) =>
            IsFarmableTarget(Pawn, bill, allowRef)
            && bill.AvailableOnNow(Pawn); //checks pawn age, sex, fertility, etc...except apparently not if male has ovum??

        private bool IsReadyForHemoFarm =>
            settings.enableFarmHemogen
            && IsFarmableNow(Pawn, xtrHemo, billCriteria)
            && (!Pawn.BillStack.Bills?.Any((Bill x) => x.recipe == xtrHemo) ?? true)
            && SanguophageUtility.CanSafelyBeQueuedForHemogenExtraction(Pawn);
        private bool IsReadyForOvumFarm =>
            settings.enableFarmOvum
            && IsFarmableNow(Pawn, xtrOvum, billCriteria)
            && (!Pawn.BillStack.Bills?.Any((Bill x) => x.recipe == xtrOvum) ?? true)
            && (!Pawn.health?.hediffSet?.HasHediff(HediffDefOf.OvumExtracted) ?? true);

        private Action<Pawn, RecipeDef, bool> CheckAndQueueExtract = (Pawn, bill, readyNow) => { if (readyNow && Pawn.Spawned) HealthCardUtility.CreateSurgeryBill(Pawn, bill, null); };

        private void CheckExtractHemogen()
        {
            CheckAndQueueExtract(Pawn, xtrHemo, shouldFarmHemogen && IsReadyForHemoFarm);
        }
        private void CheckExtractOvum()
        {
            CheckAndQueueExtract(Pawn, xtrOvum, shouldFarmOvum && IsReadyForOvumFarm);
        }

        public override void CompTickInterval(int delta)
        {
            if ((ulong)DateTime.Now.Ticks - lastTick > pollInterval && Pawn.Spawned && (IsFarmableTarget(Pawn, xtrHemo, billCriteria) || IsFarmableTarget(Pawn, xtrOvum, billCriteria)))
            {
                CheckExtractHemogen();
                CheckExtractOvum();
                lastTick = (uint)DateTime.Now.Ticks;
            }
        }

        private string GizmoText(string bill)
        {
            return "BHF_GizmoPH".Translate(bill);//$"Automatically place the '{bill}' bill on this pawn whenever it can be fulfilled without harming the pawn";
        }

        private Command_Toggle MakeGizmo(string label, RecipeDef bill, string iconDefName, Func<bool> getState, Action toggleState)
        {
            var toggleCmd = new Command_Toggle();

            toggleCmd.defaultLabel = label;
            toggleCmd.defaultDesc = GizmoText(bill.label);
            toggleCmd.hotKey = null;
            toggleCmd.icon = DefDatabase<ThingDef>.GetNamed(iconDefName).uiIcon;
            toggleCmd.isActive = getState;
            toggleCmd.toggleAction = toggleState;

            return toggleCmd;
        }

        private Gizmo ExtractHemogenGizmo()
        {
            if (IsFarmableTarget(Pawn, xtrHemo, billCriteria) && settings.enableFarmHemogen)
                return MakeGizmo
                (
                    HemoGizmoLabel,
                    xtrHemo,
                    "HemogenPack",
                    () => shouldFarmHemogen,
                    () => shouldFarmHemogen = !shouldFarmHemogen
                );
            else return null;
        }

        private Gizmo ExtractOvumGizmo()
        {
            if (IsFarmableTarget(Pawn, xtrOvum, billCriteria) && settings.enableFarmOvum)
                return MakeGizmo
                (
                    OvumGizmoLabel,
                    xtrOvum,
                    "HumanOvum",
                    () => shouldFarmOvum,
                    () => shouldFarmOvum = !shouldFarmOvum
                );
            else return null;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            var baseGizmos = base.CompGetGizmosExtra();
            foreach (var gizmo in baseGizmos) yield return gizmo;

            var hemoGizmo = ExtractHemogenGizmo();
            var ovumGizmo = ExtractOvumGizmo();
            if (hemoGizmo != null) yield return hemoGizmo;
            if (ovumGizmo != null) yield return ovumGizmo;
        }
    }
}