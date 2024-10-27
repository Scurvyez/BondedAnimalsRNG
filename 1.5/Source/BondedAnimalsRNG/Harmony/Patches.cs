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
            
            for (int i = 0; i < instructionList.Count; i++)
            {
                CodeInstruction instruction = instructionList[i];

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
            if (!___pawn.RaceProps.Animal 
                || ___pawn is not { Spawned: true } 
                || ___pawn.playerSettings == null) return;
            if (!___pawn.IsHashIntervalTick(2500)) return;
            
            Pawn masterColonist = ___pawn.playerSettings.Master;
            bool hasBondedColonist = ___pawn.Map?.mapPawns?.FreeColonistsSpawned
                .Any(colonist => ___pawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, colonist)) ?? false;
            
            if (masterColonist == null) return;
            string message = null;
            
            if (BARNGSettings.OnlyBondedCapacityChanges && hasBondedColonist)
            {
                if (___pawn.health.hediffSet.hediffs.Any(hediff =>
                        HediffCollections.CapacityChangeHediffs.Contains(hediff.def))) return;
                
                HediffDef randomHediff = HediffCollections.RandomCapacityChangeHediff;
                ___pawn.health.AddHediff(randomHediff);
                message = $"{___pawn.LabelShort} has developed a special bond with their master.";
            }
            else if (!BARNGSettings.OnlyBondedCapacityChanges && !hasBondedColonist)
            {
                if (___pawn.health.hediffSet.hediffs.Any(hediff =>
                        HediffCollections.CapacityChangeHediffs.Contains(hediff.def))) return;
                
                HediffDef randomHediff = HediffCollections.RandomCapacityChangeHediff;
                ___pawn.health.AddHediff(randomHediff);
                message = $"{___pawn.LabelShort} has grown in power by their master's side.";
            }
            
            PatchesHelper.PawnCapacityChangeYears[___pawn] = GenLocalDate.DayOfYear(___pawn.Map?.Tile ?? 0);
            
            if (!PawnUtility.ShouldSendNotificationAbout(___pawn)) return;
            Messages.Message(message, ___pawn, MessageTypeDefOf.PositiveEvent);
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
                for (int i = 0; i < capMods.Count; i++)
                {
                    PawnCapacityModifier pawnCapacityModifier = capMods[i];
                    
                    if (pawnCapacityModifier.capacity != capacity) continue;
                    float adjustment = capacityOffsetComp.randomAdjustmentValue;
                    __result *= adjustment;
                }
            }
        }
        
        private static void Pawn_RelationsTrackerRelationsTrackerTick_Postfix(Pawn ___pawn)
        {
            if (!___pawn.RaceProps.Animal) return;
            if (___pawn is not { Spawned: true } || ___pawn.playerSettings == null) return;
            if (___pawn.health?.hediffSet == null) return;
            if (!___pawn.IsHashIntervalTick(2500)) return;
            
            Pawn masterColonist = ___pawn.playerSettings.Master;
            bool hasBondedColonist = ___pawn.Map?.mapPawns?.FreeColonistsSpawned
                .Any(colonist => ___pawn.relations.DirectRelationExists(PawnRelationDefOf.Bond, colonist)) ?? false;
            
            List<Hediff> hediffsToRemove = [];
            
            if (BARNGSettings.OnlyBondedCapacityChanges && !hasBondedColonist)
            {
                hediffsToRemove = ___pawn.health.hediffSet.hediffs
                    .Where(hediff => hediff is HediffWithCapacityChange).ToList();
                
                foreach (Hediff hediff in hediffsToRemove)
                {
                    ___pawn.health.RemoveHediff(hediff);
                }
            }
            else if (!BARNGSettings.OnlyBondedCapacityChanges && masterColonist == null)
            {
                hediffsToRemove = ___pawn.health.hediffSet.hediffs
                    .Where(hediff => hediff is HediffWithCapacityChange).ToList();
                
                foreach (Hediff hediff in hediffsToRemove)
                {
                    ___pawn.health.RemoveHediff(hediff);
                }
            }
        }
    }
}