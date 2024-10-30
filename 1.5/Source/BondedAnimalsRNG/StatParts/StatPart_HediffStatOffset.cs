using RimWorld;
using Verse;

namespace BondedAnimalsRNG
{
    public class StatPart_HediffStatOffset : StatPart
    {
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (req.Thing is not Pawn pawn) return;
            float adjustment = GetStatOffsetFromHediff(pawn, parentStat);
            val *= adjustment;
        }
        
        public override string ExplanationPart(StatRequest req)
        {
            if (req.Thing is not Pawn pawn) return null;
            float adjustment = GetStatOffsetFromHediff(pawn, parentStat);

            if (adjustment < 1.1f) return null;
            bool hasBondedColonist = PatchesHelper.HasBondedColonist(pawn);
            return hasBondedColonist ? $"Bonded master factor: x{adjustment:F2}" : $"Master factor: x{adjustment:F2}";
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