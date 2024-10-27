using Verse;

namespace BondedAnimalsRNG
{
    public class HediffComp_CapacityOffset : HediffComp
    {
        public HediffCompProperties_CapacityOffset Props => (HediffCompProperties_CapacityOffset)props;
        
        public float randomAdjustmentValue = 1f;
        
        public override void CompPostMake()
        {
            base.CompPostMake();
            randomAdjustmentValue = Props.adjustmentRange.RandomInRange;
        }
        
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref randomAdjustmentValue, "randomAdjustmentValue", 1f);
        }
    }
}