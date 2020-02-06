using System.Collections.Generic;
using Verse;
using RimWorld;
using Harmony;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;
using Verse.AI;

namespace BestMix
{
    public class CustomAction
    {
        public delegate void RefAction<T1, T2, T3>(ref T1 t1, T2 t2, T3 t3);
    }

    public class Debug_Validator
    {
        static IEnumerable<CodeInstruction> Transpiler_m__0(IEnumerable<CodeInstruction> instructions)
        {
            //Log.Error("Initializing test Transpile");
            var code = instructions.ToList();
            //Log.Message("Count of code instructions: "+code.Count);
            for (int i = 0; i < 16; i++)
            {
                if (code[i] == null)
                {
                    Log.Error("Code instruction " + i + " was NULL");
                    continue;
                }
                yield return code[i];
            }
            var tTP = AccessTools.Method(typeof(Debug_Validator), "testThingPawn");
            if (tTP == null)
            {
                Log.Error("Could not find method testThingPawn");
                yield break;
            }
            yield return new CodeInstruction(OpCodes.Call, tTP);
            for (int i = 12; i < code.Count; i++)
            {
                if (code[i] == null)
                {
                    Log.Error("Code instruction " + i + " was NULL");
                    continue;
                }
                yield return code[i];
            }
        }

        public static void testThingPawn(Thing t, Pawn p)
        {
            //if (t.def.defName != "RC2_Flour") return;
            //if (!(t.def.defName.StartsWith("VCEF_Raw"))) return;
            if (!(t.def.defName.StartsWith("Leather"))) return;
            //if (t.def.defName != "Leather_Wolf") return;
            System.Text.StringBuilder s = new System.Text.StringBuilder(500);
            s.Append("Looking at item " + t + ", considering pawn " + p);
            s.Append("\n  Is it forbidden to the pawn?  " + t.IsForbidden(p));
            s.Append("\n  Can the pawn reserve it?  " + p.CanReserve(t, 1, -1, null, false).ToString());
            Log.Message(s.ToString(), true);
        }
    }

    public static class Patch_WorkGiver_DoBill
    {
        static MethodBase GetBillGiverRootCell = AccessTools.Method(typeof(WorkGiver_DoBill), "GetBillGiverRootCell");
        static FieldInfo newReleventThingsFieldInfo = AccessTools.Field(typeof(WorkGiver_DoBill), "newRelevantThings");
        static CustomAction.RefAction<List<Thing>, Thing, IntVec3> refAction;
        static Thing billGiver;
        static Pawn pawn;
        static bool testFlag;
        static IntVec3 rootCell;
        public static void DoPatch(HarmonyInstance HMinstance, CustomAction.RefAction<List<Thing>, Thing, IntVec3> refAction)
        {
            var innerType = AccessTools.FirstInner(typeof(WorkGiver_DoBill), t => t.Name.Contains("AnonStorey1"));
            var innerMethod = AccessTools.FirstMethod(innerType, method => method.Name.Contains("m__3"));
            var transpiler = AccessTools.Method(typeof(Patch_WorkGiver_DoBill), "Transpiler_bracket_m__3");
            HMinstance.Patch(innerMethod, null, null, new HarmonyMethod(transpiler));

            var original = AccessTools.Method(typeof(WorkGiver_DoBill), "TryFindBestBillIngredients");
            var prefix = AccessTools.Method(typeof(Patch_WorkGiver_DoBill), "Prefix_ResourceGetter");
            HMinstance.Patch(original, new HarmonyMethod(prefix));

            Patch_WorkGiver_DoBill.refAction = refAction;
            
            var traceDebugMethod = AccessTools.FirstMethod(innerType, method => method.Name.Contains("m__0"));
            transpiler = AccessTools.Method(typeof(Debug_Validator), "Transpiler_m__0");
            HMinstance.Patch(traceDebugMethod, null, null, new HarmonyMethod(transpiler));
            

        }

        static void Prefix_ResourceGetter(WorkGiver_DoBill __instance, Pawn pawn, Thing billGiver)
        {
            Patch_WorkGiver_DoBill.billGiver = billGiver;
            Patch_WorkGiver_DoBill.pawn = pawn;
            Patch_WorkGiver_DoBill.testFlag = false;
            //if (billGiver.def.defName == "ElectricStove") testFlag = true;
            if (billGiver.def.defName == "ElectricTailoringBench") testFlag = true;
        }

        static void MethodInvoker(ref List<Thing> newReleventThings)
        {
            var rootCell = (IntVec3)GetBillGiverRootCell.Invoke(null, new object[] { billGiver, pawn });
            refAction.Invoke(ref newReleventThings, billGiver, rootCell);
        }

        static IEnumerable<CodeInstruction> Transpiler_bracket_m__3(IEnumerable<CodeInstruction> instructions)
        {
            var invokeMethod = AccessTools.Method(typeof(Patch_WorkGiver_DoBill), "MethodInvoker");
            var insts = instructions.ToList();
            int instsCount = insts.Count;
            for (int i = 0; i < instsCount; i++)
            {
                var inst = insts[i];

                if (i > 0 && i < instsCount - 1 && insts[i - 1].opcode == OpCodes.Ble && inst.opcode == OpCodes.Ldarg_0 && insts[i + 1].operand?.ToString().Contains("<>m__4") == true)
                { // entering line 739, IL, DnSpy
                    yield return new CodeInstruction(OpCodes.Ldsflda, newReleventThingsFieldInfo);
                    yield return new CodeInstruction(OpCodes.Call, invokeMethod);
                    i += 6;
                    continue; // jump to line 745, callvirt Sort. next invoking line is 746 ldsfld.
                }

                if (i > 1 && insts[i - 1].opcode == OpCodes.Stloc_0) { yield return new CodeInstruction(OpCodes.Ldloc_0); yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_WorkGiver_DoBill), "testLister")); }
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_WorkGiver_DoBill), "testLister"));
                }

                if (i > 1 && insts[i - 1].opcode == OpCodes.Ldarg_0 && insts[i].opcode == OpCodes.Ldfld &&
                    insts[i].operand.ToString().Contains("baseValidator"))
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_2);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_WorkGiver_DoBill), "checkpointOne"));
                }

                yield return inst;
            }
        }

        static void checkpointOne(Thing t)
        {
            if (Patch_WorkGiver_DoBill.testFlag)
            {
                Log.Message(("  Thing " + t + " is reachable"), true);
            }
        }

        static void testLister(List<Thing> l)
        {
            if (!Patch_WorkGiver_DoBill.testFlag) return;
            if (l == null)
            {
                Log.Error("NULL LIST");
                return;
            }
            System.Text.StringBuilder s = new System.Text.StringBuilder(500);
            s.Append("List of Things that are HaulableEver:");
            foreach (var t in l)
            {
                if (t == null) Log.Error("NULL THING");
                else s.Append("\n    " + t + " (" + t.Position + ")");
            }
            Log.Message(s.ToString(), true);
        }

        //Patch Example
        static void Patch()
        {
            DoPatch(null, Do_Work);
        }

        //Method Example
        static void Do_Work(ref List<Thing> things, Thing thing, IntVec3 intVec3)
        {

        }
        //wrote by madeline#1941
    }
}
