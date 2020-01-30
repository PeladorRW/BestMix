using System.Reflection;
using Verse;
using Harmony;

namespace BestMix
{
    [StaticConstructorOnStartup]
    static class HarmonyPatching
    {
        static HarmonyPatching()
        {
            var harmony = HarmonyInstance.Create("com.Pelador.Rimworld.BestMix");
            //HarmonyInstance.DEBUG = true;
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Patch_WorkGiver_DoBill.DoPatch(harmony, BestMixUtility.GetBMixComparer);
        }
    }
}
