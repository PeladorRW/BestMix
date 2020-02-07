using System;
using Harmony;
using System.Reflection;

namespace BestMix.Patches
{
    internal abstract class CustomHarmonyPatch
    {
        internal abstract void Patch(HarmonyInstance HMinstance);
    }
}