using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace BondedAnimalsRNG
{
    [StaticConstructorOnStartup]
    public class PatchesHelper
    {
        public static Color DrawHediffRowHelper(Color originalColor, IEnumerable<Hediff> diffs)
        {
            List<Hediff> enumerable = diffs.ToList();
            
            bool hasChange = enumerable.Any(hediff => 
                hediff.def.hediffClass == typeof(HediffWithCapacityChange) || 
                hediff.def == BARNGDefOf.BARNG_StatChange);
            
            bool hasInfection = enumerable.Any(hediff => 
                hediff.def.isInfection || hediff.def.isBad);
            
            if (hasChange && !hasInfection)
            {
                return BARNGLog.MessageMsgCol;
            }
            return originalColor;
        }
        
        public static bool IsColonyAnimalWithValidHediffSet(Pawn pawn)
        {
            return pawn.RaceProps.Animal &&
                   pawn is { Spawned: true, playerSettings: not null, health.hediffSet: not null };
        }
        
        public static bool HasBondedColonist(Pawn pawn)
        {
            return pawn.Map?.mapPawns?.FreeColonistsSpawned
                .Any(colonist => 
                    pawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, colonist)) ?? false;
        }

        public static void TryGiveInitialOrCyclicalHediff(Pawn pawn, out string messageEnd)
        {
            List<HediffDef> enabledHediffs = HediffCollections.EnabledCapacityChangeHediffs().ToList();
            HediffDef randomHediff = Rand.Chance(0.5f) && enabledHediffs.Any()
                ? enabledHediffs.RandomElement() 
                : BARNGDefOf.BARNG_StatChange;
            
            Hediff hediffInstance = pawn.health.AddHediff(randomHediff);
            messageEnd = null;
            
            if (randomHediff == BARNGDefOf.BARNG_StatChange)
            {
                if (hediffInstance.TryGetComp<HediffComp_StatOffset>() is { } statOffsetComp)
                {
                    messageEnd += $" {statOffsetComp.chosenStat.LabelCap} x{statOffsetComp.statAdjustment:F2}";
                }
            }
            else
            {
                if (hediffInstance.TryGetComp<HediffComp_CapacityOffset>() is { } capOffsetComp)
                {
                    float adjustmentValue = capOffsetComp.randomAdjustmentValue;
                    string adjustmentPercentage = (adjustmentValue * 100f - 100f).ToString("F0") + "%";
                    string sign = adjustmentValue < 1f ? "" : "+";
                    messageEnd += $" {capOffsetComp.capacityDef.LabelCap} {sign}{adjustmentPercentage}";
                }
            }
        }
    }
}