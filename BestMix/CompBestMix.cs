using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace BestMix
{
    public class CompBestMix : ThingComp
    {
        public string CurMode = "DIS";
        public bool BMixDebug = false;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<string>(ref CurMode, "CurMode", "DIS", false);
            Scribe_Values.Look<bool>(ref BMixDebug, "BMixDebug", false, false);
        }

        public override string CompInspectStringExtra()
        {
            if (BestMixUtility.IsValidForComp(parent))
            {
                string ModeDisplay = BestMixUtility.GetBMixModeDisplay(CurMode);
                return ("BestMix.CurrentMode".Translate(ModeDisplay));
            }
            return null;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }
            if (BestMixUtility.IsValidForComp(parent))
            {
                if (parent.Spawned && parent.Faction == Faction.OfPlayer)
                {
                    string BMixIconPath = BestMixUtility.GetBMixIconPath(CurMode);
                    yield return new Command_Action
                    {
                        action = delegate
                        {
                            SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
                            DoModeSelMenu();
                        },
                        hotKey = KeyBindingDefOf.Misc1,
                        defaultLabel = "BestMix.SelectModeLabel".Translate(),
                        defaultDesc = "BestMix.SelectModeDesc".Translate(),
                        icon = ContentFinder<Texture2D>.Get(BMixIconPath)
                    };
                    if (Prefs.DevMode)
                    {
                        string DebugIconPath = "UI/BestMix/DebugList";
                        yield return new Command_Toggle
                        {
                            icon = ContentFinder<Texture2D>.Get(DebugIconPath),
                            defaultLabel = "BestMix.DebugLabel".Translate(),
                            defaultDesc = "BestMix.DebugDesc".Translate(),
                            isActive = (() => (BMixDebug == true)),
                            toggleAction = delegate
                            {
                                ToggleDebug(BMixDebug);
                            }
                        };
                    }

                }
            }  
            yield break;
        }

        public void ToggleDebug(bool flag)
        {
            BMixDebug = !flag;
        }

        public void DoModeSelMenu()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();

            string text = "BestMix.DoNothing".Translate();
            list.Add(new FloatMenuOption(text, delegate
            {
                SetBMixMode(null, false);
            },
            MenuOptionPriority.Default, null, null, 29f, null));

            foreach (string mode in BestMixUtility.BMixModes())
            {
                text = BestMixUtility.GetBMixModeDisplay(mode);
                list.Add(new FloatMenuOption(text, delegate
                {
                    SetBMixMode(mode, true);
                },
                MenuOptionPriority.Default, null, null, 29f, null));
            }
            List<FloatMenuOption> sortedlist = list.OrderBy(bm => bm.Label).ToList();
            Find.WindowStack.Add(new FloatMenu(sortedlist));
        }

        public void SetBMixMode(string GizmoSel, bool edit)
        {
            if (edit)
            {
                CurMode = GizmoSel;
            }
        }
    }

    public class CompProperties_BestMix : CompProperties
    {
        public CompProperties_BestMix()
        {
            compClass = typeof(CompBestMix);
        }
    }
}

