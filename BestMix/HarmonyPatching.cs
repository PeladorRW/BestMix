using System.Reflection;
using Verse;
using Harmony;
using BestMix.Patches;

namespace BestMix
{
    [StaticConstructorOnStartup]
    static class HarmonyPatching
    {
        static HarmonyPatching()
        {
            var harmony = HarmonyInstance.Create("com.Pelador.Rimworld.BestMix");
#if DEBUG
            HarmonyInstance.DEBUG = true;
#endif
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            HarmonyPatchTool.PatchAll(harmony);
            //Patch_WorkGiver_DoBill.DoPatch(harmony, BestMixUtility.GetBMixComparer);
        }
    }
}
