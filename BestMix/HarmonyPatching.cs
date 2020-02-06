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
            BestMix.Patches.HarmonyPatchTool.PatchAll(harmony);
        }
    }
}
