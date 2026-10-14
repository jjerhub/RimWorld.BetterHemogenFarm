using System.Collections.Generic;
using System.Linq;
using Verse;
using RimWorld;
using UnityEngine;

namespace jjerhub.BetterHemogenFarmForked
{
    public static class PawnExtender
    {
        public static bool IsInfant(this Pawn pawn) => pawn.ageTracker.CurLifeStage == LifeStageDefOf.HumanlikeBaby;
        public static bool IsTeenOrOlder(this Pawn pawn) =>
            (pawn.DevelopmentalStage & DevelopmentalStage.Adult) == DevelopmentalStage.Adult;
        public static bool IsChild(this Pawn pawn) => pawn.DevelopmentalStage.Child() || pawn.DevelopmentalStage.Juvenile();
        public static bool IsSenior(this Pawn pawn)
        {
            var sets = pawn.RaceProps.hediffGiverSets;

            if (sets == null) return false;

            var ageFraction = (pawn.ageTracker.AgeBiologicalYears / pawn.RaceProps.lifeExpectancy);
            var ageFloor = sets
                .Where(s => s.hediffGivers.Any(g => g is HediffGiver_Birthday))
                .SelectMany(s => s.hediffGivers.Where(g => g is HediffGiver_Birthday))
                .Cast<HediffGiver_Birthday>()
                .Select(g => g.ageFractionChanceCurve?.Points?.FirstOrDefault().x)
                .Where(p => (p ?? 0) > 0.5)
                .Min();

            return ageFraction >= ageFloor;
        }

        public static TValue ItemOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            if (key == null) return default;

            TValue tmpValue;

            if (dict.TryGetValue(key, out tmpValue)) return tmpValue;
            else return default;
        }
    }

    public static class RectExtender
    {
        public enum RectOffsetType
        {
            Position,
            Size
        }
        public enum RectOffsetVector
        {
            X,
            Y
        }

        public static Rect OffsetSizeX(this Rect rect, float value) => rect.Offset(value, RectOffsetType.Size, RectOffsetVector.X);

        public static Rect Offset(this Rect rect, float value, RectOffsetType type, RectOffsetVector vector)
        {
            switch (type)
            {
                case RectOffsetType.Position:
                    switch (vector)
                    {
                        case RectOffsetVector.X:
                            return new Rect(rect.x + value, rect.y, rect.width, rect.height);
                        case RectOffsetVector.Y:
                            return new Rect(rect.x, rect.y + value, rect.width, rect.height);
                    }
                    break;
                case RectOffsetType.Size:
                    switch (vector)
                    {
                        case RectOffsetVector.X:
                            return new Rect(rect.x, rect.y, rect.width + value, rect.height);
                        case RectOffsetVector.Y:
                            return new Rect(rect.x, rect.y, rect.width, rect.height + value);
                    }
                    break;
            }
            return rect;
        }
    }
}