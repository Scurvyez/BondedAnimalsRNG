using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace BondedAnimalsRNG
{
    [StaticConstructorOnStartup]
    public class PatchesHelper
    {
        public static readonly Dictionary<Pawn, int> PawnCapacityChangeYears  = new();
        
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
    }
}