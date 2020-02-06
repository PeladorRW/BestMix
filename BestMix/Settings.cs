using System;
using UnityEngine;
using Verse;

namespace BestMix
{
    public class Settings : ModSettings
    {
        public void DoWindowContents(Rect canvas)
        {
            float gap = 12f;
            Listing_Standard listing_Standard = new Listing_Standard();
            listing_Standard.ColumnWidth = canvas.width;
            listing_Standard.Begin(canvas);
            listing_Standard.Gap(gap);
            checked
            {
                listing_Standard.CheckboxLabeled("BestMix.AllowBestMix".Translate(), ref AllowBestMix, null);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("BestMix.AllowMealMakersOnly".Translate(), ref AllowMealMakersOnly, null);
                listing_Standard.Gap(gap);
                listing_Standard.CheckboxLabeled("BestMix.IncludeRegionLimiter".Translate(), ref IncludeRegionLimiter, null);
                listing_Standard.Gap(gap);

                listing_Standard.End();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref AllowBestMix, "AllowBestMix", true, false);
            Scribe_Values.Look<bool>(ref AllowMealMakersOnly, "AllowMealMakersOnly", true, false);
            Scribe_Values.Look<bool>(ref IncludeRegionLimiter, "IncludeRegionLimiter", true, false);
        }

        public bool AllowBestMix = true;
        public bool AllowMealMakersOnly = true;
        public bool IncludeRegionLimiter = true;

    }
}

