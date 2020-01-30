using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.AI;

namespace BestMix
{
    public class BestMixUtility
    {
        public static bool IsValidForComp(Thing thing)
        {
            if ((Controller.Settings.AllowBestMix) && (thing is Building bChk) && ((thing as Building).def.inspectorTabs.Contains(typeof(ITab_Bills))))
            {
                if ((!(Controller.Settings.AllowMealMakersOnly)) || ((Controller.Settings.AllowMealMakersOnly) && ((thing as Building).def.building.isMealSource)))
                {
                    return true;
                }
            }
            return false;
        }

        public static float RNDFloat()
        {
            return Rand.Range(1f, 9999f);
        }

        public static List<string> BMixModes()
        {
            List<string> list = new List<string>();
            list.AddDistinct("DIS");
            list.AddDistinct("DTR");
            list.AddDistinct("HPT");
            list.AddDistinct("VLC");
            list.AddDistinct("VLE");
            list.AddDistinct("TMP");
            list.AddDistinct("FRZ");
            list.AddDistinct("RND");
            return list;
        }

        public static string GetBMixIconPath(string BMixMode)
        {
            string BMixIconPath = "UI/BestMix/";
            
            switch (BMixMode)
            {
                case "DIS": BMixIconPath += "Nearest"; break;
                case "DTR": BMixIconPath += "Expiry"; break;
                case "HPT": BMixIconPath += "Damaged"; break;
                case "VLC": BMixIconPath += "Cheapest"; break;
                case "VLE": BMixIconPath += "Value"; break;
                case "TMP": BMixIconPath += "Warmest"; break;
                case "FRZ": BMixIconPath += "Coldest"; break;
                case "RND": BMixIconPath += "Random"; break;
                default: BMixIconPath += "Nearest"; break;
            }
            
            return BMixIconPath;
        }

        public static string GetBMixModeDisplay(string BMixMode)
        {
            string ModeDisplay;
            switch (BMixMode)
            {
                case "DIS": ModeDisplay = "BestMix.ModeDistanceDIS".Translate(); break;
                case "DTR": ModeDisplay = "BestMix.ModeDaysToRotDTR".Translate(); break;
                case "HPT": ModeDisplay = "BestMix.ModeHealthHPT".Translate(); break;
                case "VLC": ModeDisplay = "BestMix.ModeValueVLC".Translate(); break;
                case "VLE": ModeDisplay = "BestMix.ModeValueVLE".Translate(); break;
                case "RND": ModeDisplay = "BestMix.ModeRandomRND".Translate(); break;
                case "TMP": ModeDisplay = "BestMix.ModeTemperatureTMP".Translate(); break;
                case "FRZ": ModeDisplay = "BestMix.ModeTemperatureFRZ".Translate(); break;
                default: ModeDisplay = "BestMix.ModeDistanceDIS".Translate(); break;
            }
            return ModeDisplay;
        }

        public static void GetBMixComparer(ref List<Thing> listToSort, Thing billGiver, IntVec3 rootCell)
        {
            string BMixMode = "DIS";
            if (Controller.Settings.AllowBestMix)
            {
                if (billGiver is Building b)
                {
                    CompBestMix compBM = billGiver.TryGetComp<CompBestMix>();
                    if (compBM != null)
                    {
                        BMixMode = compBM.CurMode;
                        if (Controller.Settings.AllowMealMakersOnly)
                        {
                            if (!((billGiver.def?.building != null && billGiver.def.building.isMealSource)))
                            {
                                BMixMode = "DIS";
                            }
                        }
                    }
                }
            }

            Log.Message("Mix Mode: " + BMixMode); // debug
            Comparison<Thing> comparison = null;
            switch (BMixMode)
            {
                case "DIS":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (t1.Position - rootCell).LengthHorizontalSquared;
                        float value = (t2.Position - rootCell).LengthHorizontalSquared;
                        return (num.CompareTo(value));
                    };
                    break;
                case "DTR":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float maxdtr = 72000000;
                        float t1dtr = maxdtr;
                        CompRottable t1comp = t1.TryGetComp<CompRottable>();
                        if (t1comp != null)
                        {
                            t1dtr = t1comp.TicksUntilRotAtCurrentTemp;
                        }
                        float t2dtr = maxdtr;
                        CompRottable t2comp = t2.TryGetComp<CompRottable>();
                        if (t2comp != null)
                        {
                            t2dtr = t2comp.TicksUntilRotAtCurrentTemp;
                        }
                        float num = (maxdtr - t1dtr);
                        float value = (maxdtr - t2dtr);
                        return (num.CompareTo(value));
                    };
                    break;
                case "HPT":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = 1f;
                        if (t1.def.useHitPoints)
                        {
                            num = (t1.MaxHitPoints - t1.HitPoints) / t1.MaxHitPoints;
                        }
                        float value = 1f;
                        if (t2.def.useHitPoints)
                        {
                            value = (t2.MaxHitPoints - t2.HitPoints) / t2.MaxHitPoints;
                        }
                        return (num.CompareTo(value));
                    };
                    break;
                case "VLC":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float maxVal = 99999999;
                        float num = (maxVal - t1.MarketValue);
                        float value = (maxVal - t2.MarketValue);
                        return (num.CompareTo(value));
                    };
                    break;
                case "VLE":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t1.MarketValue;
                        float value = t2.MarketValue;
                        return (num.CompareTo(value));
                    };
                    break;
                case "TMP":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t1.AmbientTemperature;
                        float value = t1.AmbientTemperature;
                        return (num.CompareTo(value));
                    };
                    break;
                case "FRZ":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float maxVal = 999999;
                        float num = (maxVal - t1.AmbientTemperature);
                        float value = (maxVal - t1.AmbientTemperature);
                        return (num.CompareTo(value));
                    };
                    break;
                case "RND":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (t1.thingIDNumber / t1.thingIDNumber) * RNDFloat();
                        float value = (t1.thingIDNumber / t1.thingIDNumber) * RNDFloat();
                        return (num.CompareTo(value));
                    };
                    break;
                default:
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (t1.Position - rootCell).LengthHorizontalSquared;
                        float value = (t2.Position - rootCell).LengthHorizontalSquared;
                        return (num.CompareTo(value));
                    };
                    break;
            }
            listToSort.Sort(comparison);
            return;
        }

    }
}

