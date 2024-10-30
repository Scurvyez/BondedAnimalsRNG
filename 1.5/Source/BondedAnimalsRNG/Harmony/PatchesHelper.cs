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
            bool hasChange = enumerable.Any(hediff => hediff.def.hediffClass == typeof(HediffWithCapacityChange) || 
                                                      hediff.def == BARNGDefOf.BARNG_StatChange);
            bool hasInfection = enumerable.Any(hediff => hediff.def.isInfection || hediff.def.isBad);
            
            if (hasChange && !hasInfection)
            {
                return BARNGLog.MessageMsgCol;
            }
            return originalColor;
        }
        
        public static bool IsColonyAnimalWithValidHediffSet(Pawn pawn)
        {
            return pawn.RaceProps.Animal &&
                   pawn is { Spawned: true, playerSettings: not null, health.hediffSet: not null } &&
                   pawn.IsHashIntervalTick(2500);
        }
        
        public static bool HasBondedColonist(Pawn pawn)
        {
            return pawn.Map?.mapPawns?.FreeColonistsSpawned
                .Any(colonist => pawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, colonist)) ?? false;
        }
    }
}