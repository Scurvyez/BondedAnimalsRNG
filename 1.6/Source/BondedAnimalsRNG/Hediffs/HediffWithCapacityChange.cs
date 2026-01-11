using System.Linq;
using UnityEngine;
using Verse;

namespace BondedAnimalsRNG
{
    public class HediffWithCapacityChange : HediffWithComps
    {
        public override Color LabelColor => BARNGLog.MessageMsgCol;
        
        public override string Label
        {
            get
            {
                string adjustmentPercentage = "BARNG_NA".Translate();
                float adjustmentValue = 0f;
                
                foreach (HediffComp comp in comps)
                {
                    if (comp is not HediffComp_CapacityOffset capacityOffsetComp) continue;
                    
                    adjustmentValue = capacityOffsetComp.randomAdjustmentValue;
                    adjustmentPercentage = (adjustmentValue * 100f - 100f).ToString("F0") + "%";
                    break;
                }
                
                string sign = adjustmentValue < 1f ? "" : "+";
                return $"{def.stages.ElementAt(0).capMods.ElementAt(0).capacity.label} {sign}{adjustmentPercentage}";
            }
        }
    }
}