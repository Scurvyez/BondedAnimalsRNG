using RimWorld;
using Verse;

namespace BondedAnimalsRNG
{
    public class HediffComp_StatOffset : HediffComp
    {
        public HediffCompProperties_StatOffset Props => (HediffCompProperties_StatOffset)props;
        
        public StatDef chosenStat;
        public float statAdjustment;
        
        public override void CompPostMake()
        {
            base.CompPostMake();
            
            if (Props.hediffCompData != null && Props.hediffCompData.Any())
            {
                HediffCompData randomData = Props.hediffCompData.RandomElement();
                chosenStat = randomData.randomStat;
                statAdjustment = randomData.statAdjustmentRange.RandomInRange;
            }
        }
        
        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            chosenStat = null;
            statAdjustment = 0f;
        }
        
        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Defs.Look(ref chosenStat, "chosenStat");
            Scribe_Values.Look(ref statAdjustment, "statAdjustment", 1f);
        }
    }
}