using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace BondedAnimalsRNG
{
    public class StatPart_MasterActive : StatPart
    {
        private float _adjustedStat;
        
        public override void TransformValue(StatRequest req, ref float val)
        {
            if (!ActiveFor(req.Thing)) return;
            HediffComp_StatOffset statOffsetComp = GetStatOffsetComp(req.Thing as Pawn);
            
            if (statOffsetComp == null) return;
            if (val == 0f)
            {
                val = 1f;
            }
            
            if (Mathf.Approximately(_adjustedStat, 1f))
            {
                _adjustedStat = statOffsetComp.statAdjustment;
            }
            val *= _adjustedStat;
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (!req.HasThing || !ActiveFor(req.Thing)) return null;
            StringBuilder explanationBuilder = new ();
            
            explanationBuilder.AppendLine(
                "StatsReport_MultiplierFor".Translate() 
                + ": x" + _adjustedStat.ToStringPercent()
            );
            
            return explanationBuilder.Length > 0 ? explanationBuilder.ToString() : null;
        }
        
        private bool ActiveFor(Thing t)
        {
            Pawn pawn = t as Pawn;
            return pawn is { Map: not null, Position.IsValid: true, playerSettings.RespectedMaster: not null };
        }

        private HediffComp_StatOffset GetStatOffsetComp(Pawn pawn)
        {
            Hediff hediff = pawn?.health.hediffSet.GetFirstHediffOfDef(BARNGDefOf.BARNG_StatChange);
            return hediff?.TryGetComp<HediffComp_StatOffset>();
        }
    }
}