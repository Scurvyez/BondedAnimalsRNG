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
            bool hasCapacityChange = enumerable.Any(hediff => hediff.def.hediffClass == typeof(HediffWithCapacityChange));
            bool hasInfection = enumerable.Any(hediff => hediff.def.isInfection || hediff.def.isBad);
            
            if (hasCapacityChange && !hasInfection)
            {
                return BARNGLog.MessageMsgCol;
            }
            return originalColor;
        }
        
        public static bool IsColonyAnimalWithValidHediffSet(Pawn pawn)
        {
            return pawn.RaceProps.Animal &&
                   pawn is { Spawned: true } &&
                   pawn.playerSettings != null &&
                   pawn.health?.hediffSet != null &&
                   pawn.IsHashIntervalTick(2500);
        }
        
        public static bool HasBondedColonist(Pawn pawn)
        {
            return pawn.Map?.mapPawns?.FreeColonistsSpawned
                .Any(colonist => pawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, colonist)) ?? false;
        }
    }
}