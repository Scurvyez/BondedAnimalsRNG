using RimWorld;
using UnityEngine;
using Verse;

namespace BondedAnimalsRNG
{
    public class HediffWithStatChange : HediffWithComps
    {
        public override Color LabelColor => BARNGLog.MessageMsgCol;
        
        public override string Label
        {
            get
            {
                string adjustmentPercentage = "N/A";
                StatDef stat = null;
                
                foreach (HediffComp comp in comps)
                {
                    if (comp is not HediffComp_StatOffset statOffsetComp) continue;
                    stat = statOffsetComp.chosenStat;
                    float adjustmentValue = statOffsetComp.statAdjustment;
                    adjustmentPercentage = $"x{adjustmentValue:F2}";
                    break;
                }
                
                return $"{stat?.label} {adjustmentPercentage}";
            }
        }
    }
}