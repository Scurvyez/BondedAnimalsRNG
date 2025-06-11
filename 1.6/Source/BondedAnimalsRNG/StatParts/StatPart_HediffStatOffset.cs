using RimWorld;
using UnityEngine;
using Verse;

namespace BondedAnimalsRNG
{
    public class StatPart_HediffStatOffset : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.Thing is not Pawn pawn) return;
            float adjustment = GetStatOffsetFromHediff(pawn, parentStat);
            
            if (Mathf.Approximately(val, 0f))
            {
                val = 1f;
                val *= adjustment;
            }
            else
            {
                val *= adjustment;   
            }
        }
        
        public override string ExplanationPart(StatRequest req)
        {
            if (req.Thing is not Pawn pawn) return null;
            float adjustment = GetStatOffsetFromHediff(pawn, parentStat);
            
            if (adjustment < 1.1f) return null;
            bool hasBondedColonist = PatchesHelper.HasBondedColonist(pawn);
            return hasBondedColonist
                ? "BARNG_BondedMasterFactor".Translate() + $"x{adjustment:F2}"
                : "BARNG_MasterFactor".Translate() + $"x{adjustment:F2}";
        }
        
        private static float GetStatOffsetFromHediff(Pawn pawn, StatDef statDef)
        {
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                if (hediff.TryGetComp<HediffComp_StatOffset>() is { } comp
                    && comp.chosenStat == statDef)
                {
                    return comp.statAdjustment;
                }
            }
            return 1f;
        }
    }
}