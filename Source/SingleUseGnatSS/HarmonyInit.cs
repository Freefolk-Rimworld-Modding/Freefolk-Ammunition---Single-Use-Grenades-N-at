using HarmonyLib;
using Verse;

namespace SingleUseGnatSS
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            Harmony harmonyInstance = new Harmony("Freefolk.Ammunition.SingleUseGNATSS");
            harmonyInstance.PatchAll();
            Log.Message("[SingleUseGNAT]Simple sidearms detected, Harmony Patching SingleUseGNAT.Verb_LaunchProjectileOneUse");
        }
    }
}
