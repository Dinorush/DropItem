using DropItem.Comps;
using HarmonyLib;
using Il2CppInterop.Runtime;
using LevelGeneration;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DropItem.Inject
{
    [HarmonyPatch(typeof(SNet_SyncManager), nameof(SNet_SyncManager.OnRecallDone))]
    internal static class Inject_SNet_SyncManager
    {
        private static void Postfix()
        {
            var slots = GameObject.FindObjectsOfTypeAll(Il2CppType.Of<ResourceContainerSlot>());
            foreach (var slot in slots)
            {
                var s = slot.Cast<ResourceContainerSlot>();
                s.RemoveAllItem();
            }

            var syncs = GameObject.FindObjectsOfTypeAll(Il2CppType.Of<LG_PickupItem_Sync>());
            foreach (var comp in syncs)
            {
                var sync = comp.TryCast<LG_PickupItem_Sync>();
                if (sync == null)
                    continue;

                var item = sync.item;
                if (!ResourceContainerSlot.IsValidItemForSlot(item))
                    continue;

                var state = sync.GetCurrentState();
                if (state.status == ePickupItemStatus.PlacedInLevel)
                {
                    if (ResourceContainerSlot.TryFindSlotInPosition(state.placement.position, out var slot))
                    {
                        slot.AddItem(item.gameObject.GetInstanceID());
                    }
                }
            }

            Logger.Error("Recalled!");
        }
    }
}
