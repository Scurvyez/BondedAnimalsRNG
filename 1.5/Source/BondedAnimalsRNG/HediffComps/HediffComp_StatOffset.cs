using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BondedAnimalsRNG
{
    public class HediffComp_StatOffset : HediffComp
    {
        public HediffCompProperties_StatOffset Props => (HediffCompProperties_StatOffset)props;

        public float statAdjustment;
        
        public override void CompPostMake()
        {
            base.CompPostMake();
            statAdjustment = Rand.Range(0.1f, 2f);
            StatDef chosenStatDef = Props.possibleStatDefs.RandomElement();
            BARNGLog.Message($"[HediffComp_StatOffset.CompPostMake] Chosen StatDef: {chosenStatDef}, Stat Adjustment: {statAdjustment}");
            AddStatPartTo(chosenStatDef);
        }
        
        private void AddStatPartTo(StatDef statDef)
        {
            statDef.parts ??= [];
            statDef.parts.Add(new StatPart_MasterActive());
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref statAdjustment, "statAdjustment", 1f);
        }
    }
}