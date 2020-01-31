using System.Collections.Generic;
using Verse;
using RimWorld;
using Harmony;
using System.Reflection;
using System.Linq;
using System.Reflection.Emit;

namespace BestMix
{
    public class CustomAction
    {
        public delegate void RefAction<T1, T2, T3>(ref T1 t1, T2 t2, T3 t3);
    }

    public static class Patch_WorkGiver_DoBill
    {
        static MethodBase GetBillGiverRootCell = AccessTools.Method(typeof(WorkGiver_DoBill), "GetBillGiverRootCell");
        static FieldInfo newReleventThingsFieldInfo = AccessTools.Field(typeof(WorkGiver_DoBill), "newRelevantThings");
        static CustomAction.RefAction<List<Thing>, Thing, IntVec3> refAction;
        static Thing billGiver;
        static Pawn pawn;
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
        }

        static void Prefix_ResourceGetter(Pawn pawn, Thing billGiver)
        {
            Patch_WorkGiver_DoBill.billGiver = billGiver;
            Patch_WorkGiver_DoBill.pawn = pawn;
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

                yield return inst;
            }
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
