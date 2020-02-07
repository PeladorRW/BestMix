using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

using Harmony;

using RimWorld;
using Verse;

namespace BestMix.Patches
{
    class Patch_WorkGiver_DoBill : CustomHarmonyPatch
    {
        internal override void Patch(HarmonyInstance HMinstance)
        {
            var original = AccessTools.Method(typeof(WorkGiver_DoBill), "TryFindBestBillIngredients");
            var transpiler = AccessTools.Method(typeof(Patch_WorkGiver_DoBill), "Transpiler_TryFindBestBillIngredients");
            HMinstance.Patch(original, null, null, new HarmonyMethod(transpiler));
        }
        
        static IEnumerable<CodeInstruction> Transpiler_TryFindBestBillIngredients(IEnumerable<CodeInstruction> instructions)
        {
            Type workGiverType = typeof(WorkGiver_DoBill);
            FieldInfo RegionProcessorSubtitutionSingleton = AccessTools.Field(typeof(RegionProcessorSubtitution), "singleton");
            var LdvirtftnMethodBase = AccessTools.Method(typeof(RegionProcessorSubtitution), "RegionProcessor");
            var RegionProcessorType = AccessTools.TypeByName("RegionProcessor"); // hidden type
            var RegionProcessorPointerCtor = AccessTools.Constructor(RegionProcessorType, new Type[] { typeof(object), typeof(IntPtr)});
            //does nameof() can make an error? IDK
            MethodInfo FetchLocalFields = AccessTools.Method(typeof(RegionProcessorSubtitution), RegionProcessorSubtitution.FetchLocalFieldsMethodName);
            MethodInfo FetchStaticFields = AccessTools.Method(typeof(RegionProcessorSubtitution), RegionProcessorSubtitution.FetchStaticFieldsMethodName);
            //h = hidden type
            var c__AnonStorey1 = AccessTools.FirstInner(workGiverType, type => type.Name.Contains("AnonStorey1"));
            var h_adjacentRegionsAvailable = AccessTools.Field(c__AnonStorey1, "adjacentRegionsAvailable");
            var h_pawn = AccessTools.Field(c__AnonStorey1, "pawn");
            var h_regionsProcessed = AccessTools.Field(c__AnonStorey1, "regionsProcessed");
            var h_rootCell = AccessTools.Field(c__AnonStorey1, "rootCell");
            var h_foundAll = AccessTools.Field(c__AnonStorey1, "foundAll");
            var h_bill = AccessTools.Field(c__AnonStorey1, "bill");
            var h_billGiver = AccessTools.Field(c__AnonStorey1, "billGiver");
            var h_chosen = AccessTools.Field(c__AnonStorey1, "chosen");
            //sf = static field
            var sf_chosenIngThings = AccessTools.Field(workGiverType, "chosenIngThings");
            var sf_relevantThings = AccessTools.Field(workGiverType, "relevantThings");
            var sf_processedThings = AccessTools.Field(workGiverType, "processedThings");
            var sf_newRelevantThings = AccessTools.Field(workGiverType, "newRelevantThings");
            var sf_ingredientsOrdered = AccessTools.Field(workGiverType, "ingredientsOrdered");

            List<CodeInstruction> insts = instructions.ToList();
            int instsLength = insts.Count;
            for(int i = 0; i < instsLength; i++)
            {
                var inst = insts[i];
                #region data field fetcher patch section
                if(i < instsLength - 1 && inst.opcode == OpCodes.Ldloc_0 && insts[i+1].opcode == OpCodes.Ldftn)
                { // entering IL_01A6 ldloc.0, line 1870
                    #region local data field fetcher section
                    //prepare for Fetchdata method parameters
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_adjacentRegionsAvailable); // index 0
                    
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_regionsProcessed); // index 1
                    
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_rootCell); // index 2

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_bill); // index 3

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_pawn); // index 4

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_billGiver); // index 5

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, h_chosen); // index 6

                    yield return new CodeInstruction(OpCodes.Ldc_I4_0); // index 7

                    yield return new CodeInstruction(OpCodes.Ldsfld, RegionProcessorSubtitutionSingleton);
                    yield return new CodeInstruction(OpCodes.Call, FetchLocalFields);
                    #endregion

                    #region static data field fetcher section
                    //prepare for FetchStaticFields parameters
                    yield return new CodeInstruction(OpCodes.Ldsfld, sf_chosenIngThings);
                    yield return new CodeInstruction(OpCodes.Ldsfld, sf_relevantThings);
                    yield return new CodeInstruction(OpCodes.Ldsfld, sf_processedThings);
                    yield return new CodeInstruction(OpCodes.Ldsfld, sf_newRelevantThings);
                    yield return new CodeInstruction(OpCodes.Ldsfld, sf_ingredientsOrdered);
                    
                    yield return new CodeInstruction(OpCodes.Ldsfld, RegionProcessorSubtitutionSingleton);
                    yield return new CodeInstruction(OpCodes.Call, FetchStaticFields);
                    #endregion
                }
                #endregion

                if(i < instsLength - 1 && inst.opcode == OpCodes.Ldloc_0 && insts[i+1].opcode == OpCodes.Ldftn)
                { // entering IL_01A6 ldloc.0, line 1870
                    yield return new CodeInstruction(OpCodes.Ldsfld, RegionProcessorSubtitutionSingleton);
                    yield return new CodeInstruction(OpCodes.Ldvirtftn, LdvirtftnMethodBase);
                    yield return new CodeInstruction(OpCodes.Newobj, RegionProcessorPointerCtor);
                    i += 2; // jump to IL_01AD, newobj, line 1873
                    continue; // next line is IL_01B2, stloc.1, line 1873
                }

                if(i > 0 && inst.opcode == OpCodes.Ldsfld && insts[i-1].opcode == OpCodes.Call)
                { // entering IL_01CB, ldsfld, line 1882
                    //TODO : put UpdateData method in here.
                    //TODO : put foundall if statement here. as IL codes.
                    //if(i <)
                }


                yield return inst;
            }


            throw new NotImplementedException();
        }
    }
}

/*
1) ldvirtftn으로 함수 포인터를 스택에 적재
2) newobj RegionProcessor::.ctor를 통해서 RegionProcessor 객체 생성 -> 스택 적재
*/