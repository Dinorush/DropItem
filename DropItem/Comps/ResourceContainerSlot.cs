using DropItem.Handlers;
using GameData;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using LevelGeneration;
using Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DropItem.Comps
{
    internal sealed class ResourceContainerSlot : MonoBehaviour
    {
        private static readonly Dictionary<IntPtr, ResourceContainerSlot> _CreatedSlots = new();

        public Il2CppReferenceField<BoxCollider> Collider;
        public Il2CppReferenceField<LG_WeakResourceContainer_Graphics> Graphic;
        public Il2CppReferenceField<LG_ResourceContainer_Storage> Storage;
        public Il2CppReferenceField<StorageSlot> Slot;
        public Il2CppValueField<int> SlotIndex;
        public readonly Dictionary<IntPtr, LG_PickupItem_Sync> SlotItems = new(4);

        [HideFromIl2Cpp]
        public void Setup(LG_WeakResourceContainer_Graphics graphic, LG_ResourceContainer_Storage storage, int slotIndex)
        {
            if (graphic == null)
            {
                Destroy(this);
                Logger.Error($"Slot was invalid::Graphic was null! Destroying...: {transform.parent.name} Idx: {slotIndex}");
            }

            if (storage == null)
            {
                Destroy(this);
                Logger.Error($"Slot was invalid::Storage was null! Destroying...: {transform.parent.name} Idx: {slotIndex}");
            }

            if (storage.m_storageSlots.Length <= slotIndex)
            {
                Destroy(this);
                Logger.Error($"Slot was invalid::Index Exceed Capacity! Destroying...: {transform.parent.name} {slotIndex} of {storage.m_storageSlots.Length}");
            }

            var slot = storage.m_storageSlots[slotIndex];
            if (slot == null)
            {
                Destroy(this);
                Logger.Error($"Slot was invalid::Slot Was null? Destroying...: {transform.parent.name} {slotIndex}");
            }

            Graphic.Set(graphic);
            Storage.Set(storage);
            Slot.Set(slot);
            SlotIndex.Set(slotIndex);
            _CreatedSlots.Add(Pointer, this);
        }

        public void OnDestroy()
        {
            _CreatedSlots.Remove(Pointer);
        }

        [HideFromIl2Cpp]
        public bool IsContainerOpened()
        {
            return Graphic.Value.m_status == eResourceContainerStatus.Open;
        }

        [HideFromIl2Cpp]
        public bool GetIsSlotInUse()
        {
            return SlotItems.Count > 0;
        }

        [HideFromIl2Cpp]
        public void AddItem(LG_PickupItem_Sync sync)
        {
            SlotItems.Add(sync.gameObject.Pointer, sync);
        }

        [HideFromIl2Cpp]
        public void RemoveItem(LG_PickupItem_Sync sync)
        {
            SlotItems.Remove(sync.gameObject.Pointer);
        }

        [HideFromIl2Cpp]
        public void RemoveAllItem()
        {
            SlotItems.Clear();
        }

        [HideFromIl2Cpp]
        public Transform GetTransform(InventorySlot slot)
        {
            return slot switch
            {
                InventorySlot.ResourcePack => Slot.Value.ResourcePack,
                InventorySlot.Consumable => Slot.Value.Consumable,
                _ => Slot.Value.Consumable
            };
        }

        [HideFromIl2Cpp]
        public static bool IsValidItemForDrop(Item item)
        {
            if (item == null)
                return false;

            if (item.ItemDataBlock == null)
                return false;

            return item.ItemDataBlock.inventorySlot switch
            {
                InventorySlot.ResourcePack or InventorySlot.Consumable => true,
                _ => false,
            };
        }

        [HideFromIl2Cpp]
        public static bool IsValidItemForSlot(Item item)
        {
            if (item == null)
                return false;

            if (item.ItemDataBlock == null)
                return false;

            if (item.ItemDataBlock.persistentID == GD.Item.ARTIFACT_Base_ItemData)
                return true;

            return item.ItemDataBlock.inventorySlot switch
            {
                InventorySlot.ResourcePack or InventorySlot.Consumable or InventorySlot.InPocket => true,
                _ => false,
            };
        }

        [HideFromIl2Cpp]
        public static bool TryFindSlotInPosition(Vector3 pos, out ResourceContainerSlot slot)
        {
            foreach (var collider in Physics.OverlapSphere(pos, 0.001f, DropItemManager.InteractionMask))
            {
                slot = collider.gameObject.GetComponent<ResourceContainerSlot>();
                if (slot != null)
                {
                    return true;
                }
            }

            slot = null;
            return false;
        }

        [HideFromIl2Cpp]
        public static void OnLevelCleanup()
        {
            foreach (ResourceContainerSlot slot in _CreatedSlots.Values)
            {
                if (slot != null)
                {
                    foreach (var syncItem in slot.SlotItems.Values)
                    {
                        if (syncItem != null)
                        {
                            Destroy(syncItem.gameObject);
                        }
                    }
                    
                    Destroy(slot.gameObject);
                }
            }

            // JFS - Each slot should be removed by their OnDestroy
            _CreatedSlots.Clear();
        }
    }
}
