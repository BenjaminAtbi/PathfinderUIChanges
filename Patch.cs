using HarmonyLib;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.Overtip;
using Kingmaker.UI.SettingsUI;
using Kingmaker.UI.Tooltip;
using Kingmaker.View;
using Kingmaker.Visual;
using System.Drawing;
using UnityEngine;

namespace UIChanges
{
    [HarmonyPatch(typeof(UnitEntityView), nameof(UnitEntityView.UpdateHighlight))]
    internal static class UnitEntityView_UpdateHighlight_Patch
    {
        static bool Prefix(UnitEntityView __instance, ref UnitMultiHighlight ___m_Highlighter, bool raiseEvent = true)
        {
            if (!Main.Enabled || __instance.EntityData == null) return true;

            if (!__instance.MouseHighlighted && !__instance.DragBoxHighlighted && !__instance.EntityData.Descriptor.State.IsDead && !__instance.EntityData.IsPlayersEnemy &&
                __instance.EntityData.IsPlayerFaction)
            {
                ___m_Highlighter.BaseColor = UnityEngine.Color.clear;

                if (raiseEvent)
                {
                    EventBus.RaiseEvent<IUnitHighlightUIHandler>(delegate (IUnitHighlightUIHandler h)
                    {
                        h.HandleHighlightChange(__instance);
                    });
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(OvertipController), nameof(OvertipController.NeedName))]
    internal static class OvertipController_NeedName_Patch
    {
        static bool Prefix(OvertipController __instance, ref UnitEntityData ___Unit, ref bool __result)
        {
            if (!Main.Enabled) return true;

            if (___Unit.IsPlayerFaction)
            {
                bool flag = SettingsRoot.Instance.ShowNamesForParty.CurrentState == SettingsEntityDropdownState.DropdownState.Never;
                __result = __instance.ObjectIsHovered && !flag;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(DescriptionWindowSmallBook), nameof(DescriptionWindowSmallBook.ShowWindow))]
    internal static class DescriptionWindowSmallBook_ShowWindow_Patch
    {
        static bool Prefix()
        {
            if (!Main.Enabled) return true;

            Main.DebugLog("window trigger");
            return true;
        }
    }
}
