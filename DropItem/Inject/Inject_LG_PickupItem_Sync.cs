﻿using DropItem.Comps;
using HarmonyLib;
using LevelGeneration;
using System.Collections;
using UnityEngine;

namespace DropItem.Inject
{
    [HarmonyPatch(typeof(LG_PickupItem_Sync))]
    internal static class Inject_LG_PickupItem_Sync
    {
        [HarmonyPatch(nameof(LG_PickupItem_Sync.Setup))]
        [HarmonyPostfix]
        private static void Post_Setup(LG_PickupItem_Sync __instance)
        {
            if (!ResourceContainerSlot.IsValidItemForSlot(__instance.item))
                return;

            var state = __instance.GetCurrentState();
            if (state.status == ePickupItemStatus.PlacedInLevel)
            {
                if (ResourceContainerSlot.TryFindSlotInPosition(state.placement.position, out var slot))
                {
                    slot.AddItem(__instance);
                }
            }
        }

        [HarmonyPatch(nameof(LG_PickupItem_Sync.OnStateChange))]
        [HarmonyPrefix]
        private static void Pre_OnStateChange(LG_PickupItem_Sync __instance, pPickupItemState newState, bool isRecall)
        {
            if (!ResourceContainerSlot.IsValidItemForSlot(__instance.item))
                return;

            if (newState.updateCustomDataOnly)
                return;

            if (isRecall)
                return;

            if (newState.status == ePickupItemStatus.PlacedInLevel)
            {
                if (ResourceContainerSlot.TryFindSlotInPosition(newState.placement.position, out var slot))
                {
                    slot.AddItem(__instance);
                }
            }
            else if (newState.status == ePickupItemStatus.PickedUp)
            {
                if (ResourceContainerSlot.TryFindSlotInPosition(__instance.transform.position, out var slot))
                {
                    slot.RemoveItem(__instance);
                }
            }
        }

        [HarmonyPatch(nameof(LG_PickupItem_Sync.OnStateChange))]
        [HarmonyPostfix]
        private static void Post_OnStateChange(LG_PickupItem_Sync __instance)
        {
            var culler = __instance.GetComponent<ItemCuller>();
            if (culler != null)
            {
                culler.CullBucket.NeedsShadowRefresh = true;
                culler.CullBucket.SetDirtyCMDBuffer();
            }
        }

        private static IEnumerator ItemUpdateRoutine(GameObject item)
        {
            var lastPosition = item.transform.position;
            while (true)
            {
                var currentPosition = item.transform.position;
                if (lastPosition != currentPosition)
                {

                }
                yield return null;
            }
        }
    }
}
