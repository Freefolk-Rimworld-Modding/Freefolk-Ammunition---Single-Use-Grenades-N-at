using HarmonyLib;
using PeteTimesSix.SimpleSidearms.Utilities;
using SingleUseGNAT;
using Verse;
using static PeteTimesSix.SimpleSidearms.Utilities.Enums;

namespace SingleUseGnatSS
{
    [HarmonyPatch(typeof(Verb_LaunchProjectileOneUse), "SelfConsume")]
    public static class Verb_LaunchProjectileOneUse_SelfConsume_Postfix
    {
        [HarmonyPostfix]
        public static void SelfConsume(Verb_LaunchProjectileOneUse __instance)
        {
            if (!(__instance.caster is Pawn)) return;
            Pawn pawn = (__instance.caster as Pawn);
            if (pawn.equipment.GetDirectlyHeldThings().Any) return;
            WeaponAssingment.equipBestWeaponFromInventoryByPreference(pawn, DroppingModeEnum.UsedUp);
        }
    }
}
