using Verse;

namespace BondedAnimalsRNG
{
    public class HediffComp_CapacityOffset : HediffComp
    {
        public HediffCompProperties_CapacityOffset Props => (HediffCompProperties_CapacityOffset)props;
        
        public float randomAdjustmentValue;
        
        public override void CompPostMake()
        {
            base.CompPostMake();
            randomAdjustmentValue = Props.adjustmentRange.RandomInRange;
        }
    }
}