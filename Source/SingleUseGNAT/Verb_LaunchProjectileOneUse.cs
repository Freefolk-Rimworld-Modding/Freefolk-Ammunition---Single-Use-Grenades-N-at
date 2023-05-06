using System.Collections.Generic;
using Verse;

namespace SingleUseGNAT
{
    public class Verb_LaunchProjectileOneUse : Verb_LaunchProjectile
    {
        public static bool boolSS = ModLister.HasActiveModWithName("Simple sidearms");
        protected override bool TryCastShot()
        {
            if (base.TryCastShot())
            {
                if (burstShotsLeft <= 1)
                {
                    SelfConsume();
                }
                return true;
            }
            if (burstShotsLeft < verbProps.burstShotCount)
            {
                SelfConsume();
            }
            return false;
        }

        public override void Notify_EquipmentLost()
        {
            base.Notify_EquipmentLost();
            if (state == VerbState.Bursting && burstShotsLeft < verbProps.burstShotCount)
            {
                SelfConsume();
            }
        }

        private void SelfConsume()
        {
            if (EquipmentSource != null && !EquipmentSource.Destroyed)
            {
                EquipmentSource.Destroy();
            }
            //if (boolSS) return;
            if (!(caster is Pawn)) return;
            Pawn pawn = caster as Pawn;
            if (pawn.equipment is null || !pawn.equipment.GetDirectlyHeldThings().NullOrEmpty()) return;
            List<Thing> pawnInv = pawn.inventory?.innerContainer?.InnerListForReading;
            if (pawnInv.NullOrEmpty()) return;
            foreach (Thing thing in pawnInv)
                if (thing.def == EquipmentSource.def)
                {
                    pawn.inventory.innerContainer.TryTransferToContainer(thing, pawn.equipment.GetDirectlyHeldThings(), 1, false);
                    return;
                }
            if (boolSS) return;
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
