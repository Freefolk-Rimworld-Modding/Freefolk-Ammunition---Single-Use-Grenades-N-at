using UnityEngine;
using Verse;

namespace SingleUseGNAT
{
    public class GNATSettings : ModSettings
    {
        public static bool reuseNeoAmmo = true;
        public override void ExposeData()
        {
            Scribe_Values.Look(ref reuseNeoAmmo, "reuseNeoAmmo");
            base.ExposeData();
        }
    }
    public class GNATMod : Mod
    {
        public GNATMod(ModContentPack content) : base(content)
        {
            GetSettings<GNATSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("FF_GNATLabel".Translate(), ref GNATSettings.reuseNeoAmmo, "FF_GNATToolTip".Translate());
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory() => "FF_GNAT_Settings".Translate();
    }
}
