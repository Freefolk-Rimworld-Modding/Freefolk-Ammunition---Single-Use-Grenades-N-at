using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SingleUseGNAT
{
    public class Bullet_Spawn : Bullet
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = Map;
            IntVec3 position = Position;
            base.Impact(hitThing, blockedByShield);
            BattleLogEntry_RangedImpact battleLogEntry_RangedImpact = new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing, equipmentDef, def, targetCoverDef);
            Find.BattleLog.Add(battleLogEntry_RangedImpact);
            NotifyImpact(hitThing, map, position);
            if (hitThing != null)
            {
                Pawn pawn;
                bool instigatorGuilty = (pawn = launcher as Pawn) == null || !pawn.Drafted;
                DamageInfo dinfo = new DamageInfo(def.projectile.damageDef, DamageAmount, ArmorPenetration, ExactRotation.eulerAngles.y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing, instigatorGuilty);
                hitThing.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_RangedImpact);
                Pawn pawn2 = hitThing as Pawn;
                if (pawn2 != null && pawn2.stances != null)
                {
                    pawn2.stances.stagger.Notify_BulletImpact(this);
                }
                if (def.projectile.extraDamages != null)
                {
                    foreach (ExtraDamage extraDamage in def.projectile.extraDamages)
                    {
                        if (Rand.Chance(extraDamage.chance))
                        {
                            DamageInfo dinfo2 = new DamageInfo(extraDamage.def, extraDamage.amount, extraDamage.AdjustedArmorPenetration(), ExactRotation.eulerAngles.y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing, instigatorGuilty);
                            hitThing.TakeDamage(dinfo2).AssociateWithLog(battleLogEntry_RangedImpact);
                        }
                    }
                }
                if (Rand.Chance(def.projectile.bulletChanceToStartFire) && (pawn2 == null || Rand.Chance(FireUtility.ChanceToAttachFireFromEvent(pawn2))))
                {
                    hitThing.TryAttachFire(def.projectile.bulletFireSizeRange.RandomInRange);
                }
                return;
            }
            if (!blockedByShield)
            {
                SoundDefOf.BulletImpact_Ground.PlayOneShot(new TargetInfo(Position, map));
                if (Position.GetTerrain(map).takeSplashes)
                {
                    FleckMaker.WaterSplash(ExactPosition, map, Mathf.Sqrt(DamageAmount) * 1f, 4f);
                }
                else
                {
                    FleckMaker.Static(ExactPosition, map, FleckDefOf.ShotHit_Dirt);
                }
                if (position.IsValid &&
                    def.projectile.preExplosionSpawnChance > 0 &&
                    def.projectile.preExplosionSpawnThingCount > 0 &&
                    def.projectile.preExplosionSpawnThingDef != null &&
                    Rand.Chance(def.projectile.preExplosionSpawnChance)
                    )
                {
                    ThingDef thingDef = def.projectile.preExplosionSpawnThingDef;
                    int count = def.projectile.preExplosionSpawnThingCount;
                    if (thingDef.IsFilth && position.Walkable(map))
                    {
                        FilthMaker.TryMakeFilth(position, map, thingDef, count);
                    }
                    else if (GNATSettings.reuseNeoAmmo)
                    {
                        Thing thing = ThingMaker.MakeThing(thingDef);
                        thing.stackCount = count;
                        thing.SetForbidden(true, false);
                        GenPlace.TryPlaceThing(thing, position, map, ThingPlaceMode.Near);
                    }
                }
            }
            if (Rand.Chance(def.projectile.bulletChanceToStartFire))
            {
                FireUtility.TryStartFireIn(Position, map, def.projectile.bulletFireSizeRange.RandomInRange);
            }
        }
        private void NotifyImpact(Thing hitThing, Map map, IntVec3 position)
        {
            BulletImpactData bulletImpactData = default;
            bulletImpactData.bullet = this;
            bulletImpactData.hitThing = hitThing;
            bulletImpactData.impactPosition = position;
            BulletImpactData impactData = bulletImpactData;
            hitThing?.Notify_BulletImpactNearby(impactData);
            int num = 9;
            for (int i = 0; i < num; i++)
            {
                IntVec3 c = position + GenRadial.RadialPattern[i];
                if (!c.InBounds(map))
                {
                    continue;
                }
                List<Thing> thingList = c.GetThingList(map);
                for (int j = 0; j < thingList.Count; j++)
                {
                    if (thingList[j] != hitThing)
                    {
                        thingList[j].Notify_BulletImpactNearby(impactData);
                    }
                }
            }
        }
    }
}
