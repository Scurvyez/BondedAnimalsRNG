using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
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
                    yield return CodeInstruction.Call(typeof(Patches), nameof(DrawHediffRowHelper));
                    continue;
                }
                yield return instruction;
            }
        }
        
        private static Color DrawHediffRowHelper(Color originalColor, IEnumerable<Hediff> diffs)
        {
            List<Hediff> enumerable = diffs.ToList();
            bool hasCapacityChange = enumerable.Any(hediff => hediff.def.hediffClass == typeof(HediffWithCapacityChange));
            bool hasInfection = enumerable.Any(hediff => hediff.def.isInfection || hediff.def.isBad);
            
            if (hasCapacityChange && !hasInfection)
            {
                return BARNGLog.MessageMsgCol;
            }
            return originalColor;
        }
        
        private static void Pawn_RelationsTrackerTick_CheckDevelopBondRelation_Postfix(Pawn ___pawn)
        {
            if (___pawn is not { Spawned: true } || ___pawn.playerSettings == null) return;
            Pawn respectedMaster = ___pawn.playerSettings.Master;

            if (!___pawn.IsHashIntervalTick(2500) || respectedMaster == null) return;
            if (___pawn.health.hediffSet.hediffs.Any(hediff => HediffCollections.CapacityChangeHediffs.Contains(hediff.def))) 
                return;
            
            HediffWithCapacityChange hediffWCC = (HediffWithCapacityChange)HediffMaker.
                MakeHediff(HediffCollections.CapacityChangeHediffs.RandomElement(), ___pawn);
                
            ___pawn.health.AddHediff(hediffWCC);
                
            if (PawnUtility.ShouldSendNotificationAbout(___pawn))
            {
                Messages.Message($"The animal {___pawn.LabelShort} has developed a special bond.", 
                    ___pawn, MessageTypeDefOf.PositiveEvent);
            }
        }
        
        private static void Pawn_RelationsTrackerRelationsTrackerTick_Postfix(Pawn ___pawn)
        {
            if (___pawn == null) return;
            if (!___pawn.IsHashIntervalTick(2500)) return;
            if (___pawn.playerSettings == null) return;
            
            Pawn respectedMaster = ___pawn.playerSettings.Master;

            if (respectedMaster != null) return;
            if (___pawn.health?.hediffSet == null) return;
            
            List<Hediff> hediffsToRemove = ___pawn.health.hediffSet.hediffs
                .Where(hediff => hediff is HediffWithCapacityChange).ToList();

            foreach (Hediff hediff in hediffsToRemove)
            {
                ___pawn.health.RemoveHediff(hediff);
            }
        }
    }
}