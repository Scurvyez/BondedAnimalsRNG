using System.Linq;
using Verse;

namespace BondedAnimalsRNG
{
    public class HediffComp_CapacityOffset : HediffComp
    {
        public HediffCompProperties_CapacityOffset Props => (HediffCompProperties_CapacityOffset)props;

        public PawnCapacityDef capacityDef;
        public float randomAdjustmentValue = 1f;
        
        public override void CompPostMake()
        {
            base.CompPostMake();
            capacityDef = parent.CapMods.ElementAt(0).capacity;
            randomAdjustmentValue = Props.adjustmentRange.RandomInRange;
        }
        
        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            capacityDef = null;
            randomAdjustmentValue = 1f;
        }
        
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Defs.Look(ref capacityDef, "capacityDef");
            Scribe_Values.Look(ref randomAdjustmentValue, "randomAdjustmentValue", 1f);
        }
    }
}