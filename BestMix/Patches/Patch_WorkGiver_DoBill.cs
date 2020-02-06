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
            var RegionProcessorSubtitutionFieldInfo = AccessTools.Field(typeof(RegionProcessorSubtitution), "singleton");
            var LdvirtftnMethodBase = AccessTools.Method(typeof(RegionProcessorSubtitution), "RegionProcessor");
            var RegionProcessorType = AccessTools.TypeByName("RegionProcessor"); // hidden type
            var RegionProcessorPointerCtor = AccessTools.Constructor(RegionProcessorType, new Type[] { typeof(object), typeof(IntPtr)});

            List<CodeInstruction> insts = instructions.ToList();
            int instsLength = insts.Count;
            for(int i = 0; i < instsLength; i++)
            {
                var inst = insts[i];

                if(true)
                {
                    //TODO : make a fetchData calling method here.
                    throw new NotImplementedException("put FetchData invoke here");
                }


                if(i < instsLength - 1 && inst.opcode == OpCodes.Ldloc_0 && insts[i+1].opcode == OpCodes.Ldftn)
                { // entering IL_01A6 ldloc.0, line 1870
                    yield return new CodeInstruction(OpCodes.Ldsfld, RegionProcessorSubtitutionFieldInfo);
                    yield return new CodeInstruction(OpCodes.Ldvirtftn, LdvirtftnMethodBase);
                    yield return new CodeInstruction(OpCodes.Newobj, RegionProcessorPointerCtor);
                    i += 2; // jump to IL_01AD, newobj, line 1873
                    continue; // next line is IL_01B2, stloc.1, line 1873
                }

                if(i > 0 && inst.opcode == OpCodes.Ldsfld && insts[i-1].opcode == OpCodes.Call)
                { // entering IL_01CB, ldsfld, line 1882
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