using DropItem.Handlers;
using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using LevelGeneration;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DropItem.Comps
{
    internal sealed class ResourceContainerSlot : MonoBehaviour
    {
        public static readonly Dictionary<int, HashSet<int>> _ItemsLookup = new();

        public Il2CppReferenceField<BoxCollider> Collider;
        public Il2CppReferenceField<LG_WeakResourceContainer_Graphics> Graphic;
        public Il2CppReferenceField<LG_ResourceContainer_Storage> Storage;
        public Il2CppReferenceField<StorageSlot> Slot;
        public Il2CppReferenceField<LG_PickupItem_Sync> SlotItem;
        public Il2CppValueField<int> SlotIndex;

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
            _ItemsLookup.Add(GetInstanceID(), new());
        }

        public void OnDestroy()
        {
            var id = GetInstanceID();
            if (_ItemsLookup.TryGetValue(id, out var list))
            {
                list.Clear();
                _ItemsLookup.Remove(id);
            }
        }

        [HideFromIl2Cpp]
        public bool IsContainerOpened()
        {
            return Graphic.Value.m_status == eResourceContainerStatus.Open;
        }

        [HideFromIl2Cpp]
        public bool GetIsSlotInUse()
        {
            var slotID = GetInstanceID();
            if (_ItemsLookup.TryGetValue(slotID, out var list))
            {
                return list.Count > 0;
            }
            return true;
        }

        [HideFromIl2Cpp]
        public void AddItem(int id)
        {
            var slotID = GetInstanceID();
            if (_ItemsLookup.TryGetValue(slotID, out var list))
            {
                list.Add(id);
            }
        }

        [HideFromIl2Cpp]
        public void RemoveItem(int id)
        {
            var slotID = GetInstanceID();
            if (_ItemsLookup.TryGetValue(slotID, out var list))
            {
                list.Remove(id);
            }
        }

        [HideFromIl2Cpp]
        public void RemoveAllItem()
        {
            var slotID = GetInstanceID();
            if (_ItemsLookup.TryGetValue(slotID, out var list))
            {
                list.Clear();
            }
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
        public static bool IsValidItemForSlot(Item item)
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
    }
}
