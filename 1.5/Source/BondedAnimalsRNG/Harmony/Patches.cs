using System.Collections.Generic;
using System.Linq;
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
        static Patches()
        {
            Harmony harmony = new (id: "rimworld.scurvyez.bondedanimalsrng");
            
            harmony.Patch(original: AccessTools.Method(typeof(HealthCardUtility), "DrawHediffRow"),
                transpiler: new HarmonyMethod(typeof(Patches), nameof(HealthCardUtilityDrawHediffRow_Transpiler)));
            
            harmony.Patch(original: AccessTools.Method(typeof(Pawn_RelationsTracker), "Tick_CheckDevelopBondRelation"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Pawn_RelationsTrackerTick_CheckDevelopBondRelation_Postfix)));
            
            harmony.Patch(original: AccessTools.Method(typeof(PawnCapacityUtility), "CalculateCapacityLevel"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(PawnCapacityUtilityCalculateCapacityLevel_Postfix)));
            
            harmony.Patch(original: AccessTools.Method(typeof(Pawn_RelationsTracker), "RelationsTrackerTick"),
                postfix: new HarmonyMethod(typeof(Patches), nameof(Pawn_RelationsTrackerRelationsTrackerTick_Postfix)));
        }
        
        public static IEnumerable<CodeInstruction> HealthCardUtilityDrawHediffRow_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = instructions.ToList();
            FieldInfo redColorField = typeof(HealthUtility).GetField("RedColor");
            
            foreach (CodeInstruction instruction in instructionList)
            {
                if (instruction.opcode == OpCodes.Ldsfld && (FieldInfo)instruction.operand == redColorField)
                {
                    yield return instruction;
                    
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return CodeInstruction.Call(typeof(PatchesHelper), nameof(PatchesHelper.DrawHediffRowHelper));
                    continue;
                }
                yield return instruction;
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
        
        public static void PawnCapacityUtilityCalculateCapacityLevel_Postfix(ref float __result, HediffSet diffSet, PawnCapacityDef capacity)
        {
            foreach (Hediff hediff in diffSet.hediffs)
            {
                if (!HediffCollections.CapacityChangeHediffs.Contains(hediff.def)) continue;
                HediffComp_CapacityOffset capacityOffsetComp = hediff.TryGetComp<HediffComp_CapacityOffset>();
                
                if (capacityOffsetComp == null) continue;
                List<PawnCapacityModifier> capMods = hediff.CapMods;
                
                if (capMods == null) continue;
                foreach (PawnCapacityModifier pawnCapacityModifier in capMods)
                {
                    if (pawnCapacityModifier.capacity != capacity) continue;
                    float adjustment = capacityOffsetComp.randomAdjustmentValue;
                    __result *= adjustment;
                }
            }
        }
        
        private static void Pawn_RelationsTrackerRelationsTrackerTick_Postfix(Pawn ___pawn)
        {
            if (!___pawn.IsHashIntervalTick(2500)) return;
            if (!PatchesHelper.IsColonyAnimalWithValidHediffSet(___pawn)) return;
            Pawn masterColonist = ___pawn.playerSettings.Master;
            bool hasBondedColonist = PatchesHelper.HasBondedColonist(___pawn);

            if (___pawn.health.hediffSet.hediffs.Any(hediff => HediffCollections.CapacityChangeHediffs.Contains(hediff.def)
                || hediff.def == BARNGDefOf.BARNG_StatChange))
            {
                bool removeHediffs = false;
                
                if (BARNGSettings.OnlyBondedChanges && !hasBondedColonist)
                {
                    if (masterColonist == null)
                    {
                        removeHediffs = true;
                    }
                }
                else if (!BARNGSettings.OnlyBondedChanges && masterColonist == null)
                {
                    removeHediffs = true;
                }
                
                if (removeHediffs)
                {
                    List<Hediff> hediffsToRemove = ___pawn.health.hediffSet.hediffs
                        .Where(hediff => hediff is HediffWithCapacityChange || 
                                         hediff.def == BARNGDefOf.BARNG_StatChange)
                        .ToList();
                    hediffsToRemove.ForEach(hediff => ___pawn.health.RemoveHediff(hediff));
                }   
            }
        }
    }
}