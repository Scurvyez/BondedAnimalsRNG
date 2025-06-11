using RimWorld;
using Verse;

namespace BondedAnimalsRNG
{
    [DefOf]
    public class BARNGDefOf
    {
        public static HediffDef BARNG_StatChange;
        
        public static HediffDef BARNG_MovingCapacityChange;
        public static HediffDef BARNG_ManipulationCapacityChange;
        public static HediffDef BARNG_SightCapacityChange;
        public static HediffDef BARNG_HearingCapacityChange;
        public static HediffDef BARNG_BreathingCapacityChange;
        public static HediffDef BARNG_BloodFiltrationCapacityChange;
        public static HediffDef BARNG_BloodPumpingCapacityChange;
        public static HediffDef BARNG_MetabolismCapacityChange;
        
        static BARNGDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BARNGDefOf));
        }
    }
}