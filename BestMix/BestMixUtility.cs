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
        public static bool BMixRegionIsInRange(Region r, Thing billGiver, Bill bill)
        {
            if (!(Controller.Settings.IncludeRegionLimiter))
            {
                return true;
            }

            if (!(IsValidForComp(billGiver)))
            {
                return true;
            }

            CompBestMix compBMix = billGiver.TryGetComp<CompBestMix>();
            if (compBMix != null)
            {
                if (compBMix.CurMode == "DIS")
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            
            List<IntVec3> cells = r?.Cells.ToList<IntVec3>();
            if ((cells != null) && (cells.Count > 0))
            {
                foreach (IntVec3 cell in cells)
                {
                    if (((float)((cell - billGiver.Position).LengthHorizontalSquared)) < ((float)(bill.ingredientSearchRadius * bill.ingredientSearchRadius)))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        public static bool BMixFinishedStatus(bool foundAll, Thing billGiver, out bool finishNow)
        {
            finishNow = true;
            if (billGiver is Pawn p)
            {
                return true;
            }
            if (IsValidForComp(billGiver))
            {
                CompBestMix compBMix = billGiver.TryGetComp<CompBestMix>();
                if (compBMix != null)
                {
                    if (!(compBMix.CurMode == "DIS"))
                    {
                        finishNow = false;
                        return true;
                    }
                }
            }
            return true;
        }

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
            list.AddDistinct("BIT");
            list.AddDistinct("BTY");
            list.AddDistinct("UGY");
            list.AddDistinct("HVY");
            list.AddDistinct("LGT");
            list.AddDistinct("FLM");
            list.AddDistinct("PTB");
            list.AddDistinct("PTS");
            list.AddDistinct("INH");
            list.AddDistinct("INC");
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
                case "BIT": BMixIconPath += "Fraction"; break;
                case "BTY": BMixIconPath += "Beauty"; break;
                case "UGY": BMixIconPath += "Duckling"; break;
                case "HVY": BMixIconPath += "Heaviest"; break;
                case "LGT": BMixIconPath += "Lightest"; break;
                case "FLM": BMixIconPath += "Ignition"; break;
                case "PTB": BMixIconPath += "ProtectBlunt"; break;
                case "PTS": BMixIconPath += "ProtectSharp"; break;
                case "INH": BMixIconPath += "InsulateHeat"; break;
                case "INC": BMixIconPath += "InsulateCold"; break;
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
                case "BIT": ModeDisplay = "BestMix.ModeFractionBIT".Translate(); break;
                case "BTY": ModeDisplay = "BestMix.ModeBeautyBTY".Translate(); break;
                case "UGY": ModeDisplay = "BestMix.ModeBeautyUGY".Translate(); break;
                case "HVY": ModeDisplay = "BestMix.ModeMassHVY".Translate(); break;
                case "LGT": ModeDisplay = "BestMix.ModeMassLGT".Translate(); break;
                case "FLM": ModeDisplay = "BestMix.ModeFlammableFLM".Translate(); break;
                case "PTB": ModeDisplay = "BestMix.ModeProtectPTB".Translate(); break;
                case "PTS": ModeDisplay = "BestMix.ModeProtectPTS".Translate(); break;
                case "INH": ModeDisplay = "BestMix.ModeInsulateINH".Translate(); break;
                case "INC": ModeDisplay = "BestMix.ModeInsulateINC".Translate(); break;
                default: ModeDisplay = "BestMix.ModeDistanceDIS".Translate(); break;
            }
            return ModeDisplay;
        }

        public static void GetBMixComparer(ref List<Thing> listToSort, Thing billGiver, IntVec3 rootCell)
        {
            string BMixMode = "DIS";
            bool BMixDebugBench = false;

            if (IsValidForComp(billGiver))
            {
                CompBestMix compBM = billGiver.TryGetComp<CompBestMix>();
                if (compBM != null)
                {
                    BMixMode = compBM.CurMode;
                    BMixDebugBench = compBM.BMixDebug;
                }
            }

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
                        float num = (maxdtr - t2dtr);
                        float value = (maxdtr - t1dtr);
                        return (num.CompareTo(value));
                    };
                    break;
                case "HPT":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = 0f;
                        if (t2.def.useHitPoints)
                        {
                            num = (((float)(t2.MaxHitPoints - t2.HitPoints)) / ((float)(Math.Max(1, t2.MaxHitPoints))));
                        }
                        float value = 0f;
                        if (t1.def.useHitPoints)
                        {
                            value = (((float)(t1.MaxHitPoints - t1.HitPoints)) / ((float)(Math.Max(1, t1.MaxHitPoints))));
                        }
                        return (num.CompareTo(value));
                    };
                    break;
                case "VLC":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (0f - t2.MarketValue);
                        float value = (0f - t1.MarketValue);
                        return (num.CompareTo(value));
                    };
                    break;
                case "VLE":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.MarketValue;
                        float value = t1.MarketValue;
                        return (num.CompareTo(value));
                    };
                    break;
                case "TMP":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.AmbientTemperature;
                        float value = t1.AmbientTemperature;
                        return (num.CompareTo(value));
                    };
                    break;
                case "FRZ":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (0f - t2.AmbientTemperature);
                        float value = (0f - t1.AmbientTemperature);
                        return (num.CompareTo(value));
                    };
                    break;
                case "BIT":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (((float)(t2.def.stackLimit)) / ((float)(Math.Max(1, t2.stackCount))));
                        float value = (((float)(t1.def.stackLimit)) / ((float)(Math.Max(1, t1.stackCount))));
                        return (num.CompareTo(value));
                    };
                    break;
                case "RND":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (((Math.Max(1, t2.def.stackLimit)) / (Math.Max(1, t2.def.stackLimit))) * RNDFloat());
                        float value = (((Math.Max(1, t1.def.stackLimit)) / (Math.Max(1, t1.def.stackLimit))) * RNDFloat());
                        return (num.CompareTo(value));
                    };
                    break;
                case "BTY":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.Beauty);
                        float value = t1.GetStatValue(StatDefOf.Beauty);
                        return (num.CompareTo(value));
                    };
                    break;
                case "UGY":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (0f - t1.GetStatValue(StatDefOf.Beauty));
                        float value = (0f - t2.GetStatValue(StatDefOf.Beauty));
                        return (num.CompareTo(value));
                    };
                    break;
                case "HVY":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.Mass);
                        float value = t1.GetStatValue(StatDefOf.Mass);
                        return (num.CompareTo(value));
                    };
                    break;
                case "LGT":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = (0f - t2.GetStatValue(StatDefOf.Mass));
                        float value = (0f - t1.GetStatValue(StatDefOf.Mass));
                        return (num.CompareTo(value));
                    };
                    break;
                case "FLM":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.Flammability);
                        float value = t1.GetStatValue(StatDefOf.Flammability);
                        return (num.CompareTo(value));
                    };
                    break;
                case "PTB":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.StuffPower_Armor_Blunt);
                        float value = t1.GetStatValue(StatDefOf.StuffPower_Armor_Blunt);
                        return (num.CompareTo(value));
                    };
                    break;
                case "PTS":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.StuffPower_Armor_Sharp);
                        float value = t1.GetStatValue(StatDefOf.StuffPower_Armor_Sharp);
                        return (num.CompareTo(value));
                    };
                    break;
                case "INH":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.StuffPower_Insulation_Heat);
                        float value = t1.GetStatValue(StatDefOf.StuffPower_Insulation_Heat);
                        return (num.CompareTo(value));
                    };
                    break;
                case "INC":
                    comparison = delegate (Thing t1, Thing t2)
                    {
                        float num = t2.GetStatValue(StatDefOf.StuffPower_Insulation_Cold);
                        float value = t1.GetStatValue(StatDefOf.StuffPower_Insulation_Cold);
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

            //list debugger
            if (BMixDebugBench)
            {
                if (listToSort.Count > 0)
                {
                    for (int i=0; i < listToSort.Count; i++)
                    {
                        Thing thing = listToSort[i];
                        string debugMsg = MakeDebugString(i, thing, billGiver, rootCell, BMixMode);
                        Log.Message(debugMsg, true);
                    }
                }
            }

            return;
        }

        public static string MakeDebugString(int indx, Thing thing, Thing billGiver, IntVec3 rootCell, string BMixMode)
        {
            float stat = 0f;
            switch(BMixMode)
            {
                case "DIS": stat = (thing.Position - rootCell).LengthHorizontalSquared; break;
                case "DTR": float maxdtr = 72000000;
                    float thingdtr = maxdtr;
                    CompRottable thingcomp = thing.TryGetComp<CompRottable>();
                    if (thingcomp != null)
                    {
                        thingdtr = thingcomp.TicksUntilRotAtCurrentTemp;
                    }
                    stat = (maxdtr - thingdtr);
                    break;
                case "HPT":
                    stat = 0f;
                    if (thing.def.useHitPoints)
                    {
                        stat = (((float)(thing.MaxHitPoints - thing.HitPoints)) / ((float)(Math.Max(1, thing.MaxHitPoints))));
                    }
                    break;
                case "VLC":
                    stat = (0f - thing.MarketValue);
                    break;
                case "VLE":
                    stat = thing.MarketValue;
                    break;
                case "TMP":
                    stat = thing.AmbientTemperature;
                    break;
                case "FRZ":
                    stat = (0f - thing.AmbientTemperature);
                    break;
                case "BIT":
                    stat = ((float)thing.def.stackLimit / (float)(Math.Max(1, thing.stackCount)));
                    break;
                case "RND":
                    stat = (((Math.Max(1, thing.def.stackLimit)) / (Math.Max(1, thing.def.stackLimit))) * RNDFloat());
                    break;
                case "BTY":
                    stat = thing.GetStatValue(StatDefOf.Beauty);
                    break;
                case "UGY":
                    stat = (0f - thing.GetStatValue(StatDefOf.Beauty));
                    break;
                case "HVY":
                    stat = thing.GetStatValue(StatDefOf.Mass);
                    break;
                case "LGT":
                    stat = (0f - thing.GetStatValue(StatDefOf.Mass));
                    break;
                case "FLM":
                    stat = thing.GetStatValue(StatDefOf.Flammability);
                    break;
                case "PTB":
                    stat = thing.GetStatValue(StatDefOf.StuffPower_Armor_Blunt);
                    break;
                case "PTS":
                    stat = thing.GetStatValue(StatDefOf.StuffPower_Armor_Sharp);
                    break;
                case "INH":
                    stat = thing.GetStatValue(StatDefOf.StuffPower_Insulation_Heat);
                    break;
                case "INC":
                    stat = thing.GetStatValue(StatDefOf.StuffPower_Insulation_Cold);
                    break;
                default:
                    stat = 0f;
                    break;
            }

            string debugPos = "(" + thing.Position.x.ToString() + ", " + thing.Position.z.ToString() + ")";
            string debugMsg = "Debug " + BMixMode + " " + indx.ToString() + " " + billGiver.ThingID + " " + thing.LabelShort + " " + debugPos + " " + stat.ToString("F2");
            return debugMsg;
        }

    }
}

