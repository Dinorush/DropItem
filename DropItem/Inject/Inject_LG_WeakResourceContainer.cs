using DropItem.Comps;
using DropItem.Handlers;
using HarmonyLib;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DropItem.Inject
{
    [HarmonyPatch(typeof(LG_WeakResourceContainer))]
    internal static class Inject_LG_WeakResourceContainer
    {
        [HarmonyPatch(nameof(LG_WeakResourceContainer.Setup))]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        private static void Post_Setup(LG_WeakResourceContainer __instance)
        {
            var graphic = __instance.m_graphics.TryCast<LG_WeakResourceContainer_Graphics>();
            if (graphic == null)
                return;

            var storage = __instance.m_storage.TryCast<LG_ResourceContainer_Storage>();
            if (storage == null)
                return;

            var sync = __instance.m_sync.TryCast<LG_ResourceContainer_Sync>();
            if (sync == null)
                return;

            var transform = __instance.transform;
            if (__instance.m_isLocker)
            {
                CreateSlot(transform, SlotInfo.LockerSlotInfo[0]).Setup(graphic, storage, 0);
                CreateSlot(transform, SlotInfo.LockerSlotInfo[1]).Setup(graphic, storage, 1);
                CreateSlot(transform, SlotInfo.LockerSlotInfo[2]).Setup(graphic, storage, 2);
                CreateSlot(transform, SlotInfo.LockerSlotInfo[3]).Setup(graphic, storage, 3);
                CreateSlot(transform, SlotInfo.LockerSlotInfo[4]).Setup(graphic, storage, 4);
                CreateSlot(transform, SlotInfo.LockerSlotInfo[5]).Setup(graphic, storage, 5);
            }
            else
            {
                CreateSlot(transform, SlotInfo.BoxSlotInfo[0]).Setup(graphic, storage, 0);
                CreateSlot(transform, SlotInfo.BoxSlotInfo[1]).Setup(graphic, storage, 1);
                CreateSlot(transform, SlotInfo.BoxSlotInfo[2]).Setup(graphic, storage, 2);
            }
        }

        private static ResourceContainerSlot CreateSlot(Transform root, SlotInfo info)
        {
            var cube = new GameObject
            {
                layer = DropItemManager.InteractionLayer
            };

            cube.transform.SetParent(root);
            cube.transform.localRotation = Quaternion.identity;
            cube.transform.localPosition = info.LocalPosition;
            cube.transform.localScale = info.LocalScale;
            var slot = cube.AddComponent<ResourceContainerSlot>();
            slot.Collider.Set(cube.AddComponent<BoxCollider>());
            return slot;
        }
    }
}
