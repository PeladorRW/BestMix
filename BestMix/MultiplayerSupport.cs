using Harmony;
using Multiplayer.API;
using System.Reflection;
using Verse;

namespace BestMix
{
    [StaticConstructorOnStartup]
    static class MultiplayerSupport
    {
        static HarmonyInstance harmony = HarmonyInstance.Create("rimworld.BestMix.multiplayersupport");

        static MultiplayerSupport()
        {
            if (!MP.enabled) return;

            // ==================== BestMix ==============================
            // Select Mode
            //MP.RegisterSyncMethod(typeof(CompBestMix), nameof(CompBestMix.DoModeSelMenu));
            MP.RegisterSyncMethod(typeof(CompBestMix), nameof(CompBestMix.SetBMixMode));
            MP.RegisterSyncMethod(typeof(CompBestMix), nameof(CompBestMix.ToggleDebug));


            // Add all Methods where there is Rand calls here
            var methods = new[] {
            AccessTools.Method(typeof(BestMixUtility), nameof(BestMixUtility.RNDFloat)),

        };
            foreach (var method in methods)
            {
                FixRNG(method);
            }
        }

        static void FixRNG(MethodInfo method)
        {
            harmony.Patch(method,
                prefix: new HarmonyMethod(typeof(MultiplayerSupport), nameof(FixRNGPre)),
                postfix: new HarmonyMethod(typeof(MultiplayerSupport), nameof(FixRNGPos))
            );
        }

        static void FixRNGPre() => Rand.PushState(Find.TickManager.TicksAbs);
        static void FixRNGPos() => Rand.PopState();

        internal static void MPLog(string from, string msg)
        {
            if (MP.enabled)
            {
                Log.Message(from + " " + msg);
            }
        }
    }
}

