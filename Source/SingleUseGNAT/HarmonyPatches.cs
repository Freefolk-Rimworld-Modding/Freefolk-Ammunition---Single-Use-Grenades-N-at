using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace SingleUseGNAT
{
    [HarmonyPatch(typeof(PawnInventoryGenerator), nameof(GenerateInventoryFor))]
    class Harmony_PawnInventoryGenerator_GenerateInventoryFor_Postfix
    {
        [HarmonyPostfix]
        public static void GenerateInventoryFor(Pawn p)
        {
            if (p?.equipment?.Primary?.def is null ||
                p.inventory?.innerContainer is null ||
                !p.equipment.Primary.def.HasModExtension<GenerateWithEquip>() ||
                p.equipment.Primary.def.GetModExtension<GenerateWithEquip>().generateEquip.NullOrEmpty()
                ) return;
            foreach (ThingDefCountRangeClass item in p.equipment.Primary.def.GetModExtension<GenerateWithEquip>().generateEquip)
            {
                if (p.equipment.Primary.Stuff != null)
                {
                    Thing thing = ThingMaker.MakeThing(item.thingDef, GenStuff.AllowedStuffsFor(item.thingDef).Any() ? p.equipment.Primary.Stuff : null);
                    thing.stackCount = item.countRange.RandomInRange;
                    p.inventory.innerContainer.TryAdd(thing);
                }
                else
                {
                    Thing thing = ThingMaker.MakeThing(item.thingDef, GenStuff.AllowedStuffsFor(item.thingDef).Any() ? GenStuff.AllowedStuffsFor(item.thingDef).RandomElement() : null);
                    thing.stackCount = item.countRange.RandomInRange;
                    p.inventory.innerContainer.TryAdd(thing);
                }
            }
        }
    }
    [HarmonyPatch(typeof(Verb_ShootOneUse), nameof(SelfConsume))]
    public static class Harmony_Verb_ShootOneUse_SelfConsume_Postfix
    {
        [HarmonyPostfix]
        public static void SelfConsume(Verb_ShootOneUse __instance)
        {
            if (!(__instance.caster is Pawn)) return;
            Pawn pawn = __instance.caster as Pawn;
            if (pawn.equipment.GetDirectlyHeldThings().Any) return;
            List<Thing> pawnInv = pawn.inventory?.innerContainer?.InnerListForReading;
            if (pawnInv.NullOrEmpty()) return;
            foreach (Thing thing in pawnInv)
                if (thing.def == __instance.EquipmentSource.def)
                {
                    pawn.inventory.innerContainer.TryTransferToContainer(thing, pawn.equipment.GetDirectlyHeldThings(), 1, false);
                    return;
                }
            foreach (Thing thing in pawnInv)
                if (thing.def.IsRangedWeapon)
                {
                    pawn.inventory.innerContainer.TryTransferToContainer(thing, pawn.equipment.GetDirectlyHeldThings(), 1, false);
                    return;
                }
            foreach (Thing thing in pawnInv)
                if (thing.def.IsWeapon)
                {
                    pawn.inventory.innerContainer.TryTransferToContainer(thing, pawn.equipment.GetDirectlyHeldThings(), 1, false);
                    return;
                }
        }
    }
}
