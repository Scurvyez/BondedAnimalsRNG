using System.Collections.Generic;
using Verse;

namespace BondedAnimalsRNG
{
    public static class HediffCollections
    {
        public static HediffDef RandomCapacityChangeHediff => CapacityChangeHediffs.Count == 0 
            ? null : CapacityChangeHediffs[Rand.Range(0, CapacityChangeHediffs.Count)];
        
        public static List<HediffDef> CapacityChangeHediffs = 
        [
            BARNGDefOf.BARNG_MovingCapacityChange,
            BARNGDefOf.BARNG_ManipulationCapacityChange,
            BARNGDefOf.BARNG_SightCapacityChange,
            BARNGDefOf.BARNG_HearingCapacityChange,
            BARNGDefOf.BARNG_BreathingCapacityChange,
            BARNGDefOf.BARNG_BloodFiltrationCapacityChange,
            BARNGDefOf.BARNG_BloodPumpingCapacityChange,
            BARNGDefOf.BARNG_MetabolismCapacityChange
        ];
    }
}