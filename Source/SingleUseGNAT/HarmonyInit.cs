using HarmonyLib;
using RimWorld;
using Verse;

namespace SingleUseGNAT
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            Harmony harmonyInstance = new Harmony("Freefolk.Ammunition.SingleUseGNAT");
            if (!ModLister.HasActiveModWithName("Simple sidearms"))
            {
                harmonyInstance.PatchAll();
                Log.Message("[SingleUseGNAT]Harmony Patching PawnInventoryGenerator");
                Log.Message("[SingleUseGNAT]Simple sidearms not detected, Harmony Patching Verb_ShootOneUse");
            }
            else
            {
                harmonyInstance.Patch(typeof(PawnInventoryGenerator).GetMethod("GenerateInventoryFor"), postfix: new HarmonyMethod(typeof(Harmony_PawnInventoryGenerator_GenerateInventoryFor_Postfix).GetMethod("GenerateInventoryFor")));
                Log.Message("[SingleUseGNAT]Harmony Patching PawnInventoryGenerator");
                Log.Message("[SingleUseGNAT]Simple sidearms detected, skipping Harmony Patch for Verb_ShootOneUse");
            }
        }
    }
}
