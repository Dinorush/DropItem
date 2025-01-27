using DropItem.Comps;
using LevelGeneration;
using Player;
using SNetwork;
using System;
using UnityEngine;

namespace DropItem.Handlers
{
    internal static class DropItemManager
    {
        public static readonly int InteractionLayer = LayerMask.NameToLayer("Debris");
        public static readonly int InteractionMask = LayerMask.GetMask("Debris");
        public static readonly int InteractionBlockMask = LayerMask.GetMask(
            "Default",
            "Default_NoGraph",
            "Default_BlockGraph",
            "Dynamic"
            );

        public static ItemEquippable WieldingItem { get; private set; }
        public static InventorySlot WieldingSlot { get; private set; } = InventorySlot.None;
        public static ResourceContainerSlot Slot { get; private set; }

        public static bool HasValidItem => WieldingItem != null && (WieldingSlot == InventorySlot.Consumable || WieldingSlot == InventorySlot.ResourcePack);
        public static bool InteractionAllowed { get; private set; } = false;

        public static event Action OnUpdated;

        public static void SetWieldingItem(ItemEquippable wieldingItem, InventorySlot slot)
        {
            if (WieldingSlot != slot)
            {
                WieldingItem = wieldingItem;
                WieldingSlot = slot;
                Update();
            }
            else
            {
                var oldPtr = IntPtr.Zero;
                var newPtr = IntPtr.Zero;
                if (WieldingItem != null) oldPtr = WieldingItem.Pointer;
                if (wieldingItem != null) newPtr = wieldingItem.Pointer;
                if (oldPtr != newPtr)
                {
                    WieldingItem = wieldingItem;
                    WieldingSlot = slot;
                    Update();
                }
            }
        }

        public static void SetDropSlot(ResourceContainerSlot slot)
        {
            var oldPtr = IntPtr.Zero;
            var newPtr = IntPtr.Zero;
            if (Slot != null) oldPtr = Slot.Pointer;
            if (slot != null) newPtr = slot.Pointer;

            if (oldPtr != newPtr)
            {
                Slot = slot;
                Update();
            }
        }

        public static bool TryGetItemInLevel(out ItemInLevel itemInLevel)
        {
            if (!HasValidItem)
            {
                itemInLevel = null;
                return false;
            }

            if (!PlayerBackpackManager.TryGetItemInLevelFromItemData(WieldingItem.Get_pItemData(), out var item))
            {
                itemInLevel = null;
                return false;
            }

            itemInLevel = item.TryCast<ItemInLevel>();
            return itemInLevel != null;
        }

        public static void DropWieldingItemToSlot()
        {
            if (!WieldingItem.AllowPlayerInteraction)
                return;

            if (TryGetItemInLevel(out var itemInLevel))
            {
                var sync = itemInLevel.GetSyncComponent();
                if (sync == null)
                    return;

                var baseTransform = Slot.GetTransform(WieldingSlot);
                var customData = sync.GetCustomData();
                var backpack = PlayerBackpackManager.LocalBackpack;
                if (backpack == null)
                    return;

                var localAmmo = PlayerBackpackManager.LocalBackpack.AmmoStorage;
                if (localAmmo == null)
                    return;

                customData.ammo = localAmmo.GetInventorySlotAmmo(WieldingSlot).AmmoInPack;
                sync.AttemptPickupInteraction(
                    type: ePickupItemInteractionType.Place,
                    player: SNet.LocalPlayer,
                    custom: customData,
                    position: baseTransform.position,
                    rotation: baseTransform.rotation,
                    node: Slot.Storage.Value.m_core.SpawnNode);
            }
        }

        private static void Update()
        {
            InteractionAllowed = IsInteractionAllowed();
            OnUpdated?.Invoke();
        }

        private static bool IsInteractionAllowed()
        {
            if (WieldingItem == null)
                return false;

            if (Slot == null)
                return false;

            var flag1 = (WieldingSlot == InventorySlot.Consumable || WieldingSlot == InventorySlot.ResourcePack);
            var flag2 = !Slot.GetIsSlotInUse();
            var flag3 = Slot.IsContainerOpened();
            var flag4 = WieldingItem.AllowPlayerInteraction;
            return flag1 && flag2 && flag3 && flag4;
        }

        public static void Clear()
        {
            InteractionAllowed = false;
            WieldingItem = null;
            WieldingSlot = InventorySlot.None;
            Slot = null;
        }
    }
}
