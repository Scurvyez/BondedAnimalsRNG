using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BondedAnimalsRNG
{
    [StaticConstructorOnStartup]
    public class Patches
    {
        private static readonly FieldInfo RedColorField = AccessTools.Field(typeof(HealthUtility), nameof(HealthUtility.RedColor));
        
        static Patches()
        {
            Harmony harmony = new (id: "rimworld.scurvyez.bondedanimalsrng");
            
            harmony.Patch(original: AccessTools.Method(
                    typeof(HealthCardUtility), "DrawHediffRow"),
                transpiler: new HarmonyMethod(typeof(Patches), nameof(HealthCardUtilityDrawHediffRow_Transpiler)));
            
            harmony.Patch(original: AccessTools.Method(
                    typeof(Pawn_RelationsTracker), "Tick_CheckDevelopBondRelation"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Pawn_RelationsTrackerTick_CheckDevelopBondRelation_Postfix)));
            
            harmony.Patch(original: AccessTools.Method(
                    typeof(PawnCapacityUtility), "CalculateCapacityLevel"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(PawnCapacityUtilityCalculateCapacityLevel_Postfix)));
            
            harmony.Patch(original: AccessTools.Method(
                    typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.RelationsTrackerTickInterval)),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Pawn_RelationsTrackerRelationsTrackerTickInterval_Postfix)));
        }
        
        public static IEnumerable<CodeInstruction> HealthCardUtilityDrawHediffRow_Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;
                
                if (instruction.opcode == OpCodes.Ldsfld && (FieldInfo)instruction.operand == RedColorField)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return CodeInstruction.Call(typeof(PatchesHelper), nameof(PatchesHelper.DrawHediffRowHelper));
                }
            }
        }
        
        private static void Pawn_RelationsTrackerTick_CheckDevelopBondRelation_Postfix(Pawn ___pawn)
        {
            if (!___pawn.IsHashIntervalTick(2500)) return;
            if (!PatchesHelper.IsColonyAnimalWithValidHediffSet(___pawn)) return;
            
            Pawn masterColonist = ___pawn.playerSettings.Master;
            bool hasBondedColonist = PatchesHelper.HasBondedColonist(___pawn);
            
            if (masterColonist == null) return;
            
            bool giveHediff = false;
            string messageStart = null;
            string messageEnd = null;
            string pawnName = ___pawn.Name != null 
                ? ___pawn.Name.ToStringShort 
                : ___pawn.LabelShort;
            
            if (BARNGSettings.OnlyBondedChanges && hasBondedColonist)
            {
                giveHediff = true;
                messageStart = "BARNG_BondedChange".Translate(pawnName);
            }
            else if (!BARNGSettings.OnlyBondedChanges)
            {
                giveHediff = true;
                messageStart = hasBondedColonist 
                    ? "BARNG_BondedChange".Translate(pawnName) 
                    : "BARNG_MasterChange".Translate(pawnName);
            }
            
            if (giveHediff)
            {
                if (___pawn.health.hediffSet.hediffs.Any(hediff =>
                        HediffCollections.CapacityChangeHediffs.Contains(hediff.def)
                        || hediff.def == BARNGDefOf.BARNG_StatChange)) return;
                    
                PatchesHelper.TryGiveInitialOrCyclicalHediff(___pawn, out messageEnd);
            }
            
            if (!PawnUtility.ShouldSendNotificationAbout(___pawn)) return;
            Messages.Message(messageStart + messageEnd, ___pawn, MessageTypeDefOf.PositiveEvent);
        }
        
        public static void PawnCapacityUtilityCalculateCapacityLevel_Postfix(ref float __result, 
            HediffSet diffSet, PawnCapacityDef capacity)
        {
            var hediffs = diffSet.hediffs;
            for (int i = 0; i < hediffs.Count; i++)
            {
                Hediff hediff = hediffs[i];
                if (!HediffCollections.CapacityChangeHediffs.Contains(hediff.def)) continue;
                
                HediffComp_CapacityOffset capacityOffsetComp = hediff.TryGetComp<HediffComp_CapacityOffset>();
                if (capacityOffsetComp == null) continue;
                
                List<PawnCapacityModifier> capMods = hediff.CapMods;
                if (capMods == null) continue;

                for (int j = 0; j < capMods.Count; j++)
                {
                    if (capMods[j].capacity == capacity)
                    {
                        __result *= capacityOffsetComp.randomAdjustmentValue;
                    }
                }
            }
        }
        
        private static void Pawn_RelationsTrackerRelationsTrackerTickInterval_Postfix(Pawn ___pawn)
        {
            if (!___pawn.IsHashIntervalTick(2000)) return;
            if (!PatchesHelper.IsColonyAnimalWithValidHediffSet(___pawn)) return;
            
            Pawn masterColonist = ___pawn.playerSettings.Master;
            bool hasBondedColonist = PatchesHelper.HasBondedColonist(___pawn);

            bool shouldRemove = false;
            if (BARNGSettings.OnlyBondedChanges)
            {
                if (!hasBondedColonist && masterColonist == null) shouldRemove = true;
            }
            else if (masterColonist == null)
            {
                shouldRemove = true;
            }

            if (shouldRemove)
            {
                List<Hediff> hediffs = ___pawn.health.hediffSet.hediffs;
                for (int i = hediffs.Count - 1; i >= 0; i--)
                {
                    Hediff hediff = hediffs[i];
                    if (hediff is HediffWithCapacityChange || hediff.def == BARNGDefOf.BARNG_StatChange)
                    {
                        ___pawn.health.RemoveHediff(hediff);
                    }
                }
            }
        }
    }
}