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
            
            if (Mathf.Approximately(adjustment, 1f)) return;
            val *= adjustment;
        }
        
        public override string ExplanationPart(StatRequest req)
        {
            if (req.Thing is not Pawn pawn) return null;
            float adjustment = GetStatOffsetFromHediff(pawn, parentStat);
            
            if (Mathf.Approximately(adjustment, 1f)) return null;
            
            bool hasBondedColonist = PatchesHelper.HasBondedColonist(pawn);
            string label = hasBondedColonist
                ? "BARNG_BondedMasterFactor".Translate()
                : "BARNG_MasterFactor".Translate();
            
            return $"{label}: x{adjustment:F2}";
        }
        
        private static float GetStatOffsetFromHediff(Pawn pawn, StatDef statDef)
        {
            var hediffs = pawn.health?.hediffSet?.hediffs;
            
            if (hediffs == null) return 1f;
            
            foreach (var hediff in hediffs)
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