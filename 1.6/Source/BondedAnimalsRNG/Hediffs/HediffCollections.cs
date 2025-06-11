using System.Collections.Generic;
using System.Linq;
using Verse;

namespace BondedAnimalsRNG
{
    public static class HediffCollections
    {
        public static IEnumerable<HediffDef> EnabledCapacityChangeHediffs()
        {
            return CapacityChangeHediffs.Where(hediff => BARNGSettings.Instance.IsHediffEnabled(hediff.defName));
        }
        
        public static readonly List<HediffDef> CapacityChangeHediffs = 
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