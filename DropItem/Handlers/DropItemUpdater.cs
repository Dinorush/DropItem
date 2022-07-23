using DropItem.Comps;
using Il2CppInterop.Runtime.Attributes;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DropItem.Handlers
{
    internal sealed class DropItemUpdater : MonoBehaviour
    {
        public LocalPlayerAgent Agent;

        [HideFromIl2Cpp]
        public Ray AgentRay => new()
        {
            origin = Agent.EyePosition,
            direction = Agent.FPSCamera.Forward
        };

        void Awake()
        {
            DropItemManager.Clear();
        }

        void OnDestroy()
        {
            DropItemManager.Clear();
        }

        void FixedUpdate()
        {
            UpdateSlotInRay();
            UpdateWieldingItem();
        }

        void UpdateSlotInRay()
        {
            if (Physics.Raycast(AgentRay, out var hit, 1.35f, DropItemManager.InteractionMask))
            {
                if (hit.collider == null)
                    goto FAILED;

                if (hit.collider.gameObject == null)
                    goto FAILED;

                var slot = hit.collider.gameObject.GetComponent<ResourceContainerSlot>();
                if (slot == null)
                    goto FAILED;

                if (slot.GetIsSlotInUse())
                    goto FAILED;

                if (!slot.IsContainerOpened())
                    goto FAILED;

                if (Physics.Linecast(Agent.EyePosition, hit.point, DropItemManager.InteractionBlockMask))
                    goto FAILED;

                DropItemManager.SetDropSlot(slot);
                return;
            }

        FAILED:
            DropItemManager.SetDropSlot(null);
        }

        void UpdateWieldingItem()
        {
            var inventory = Agent.Inventory;
            if (inventory != null)
            {
                var wieldedSlot = inventory.WieldedSlot;
                var wieldedItem = inventory.WieldedItem;
                DropItemManager.SetWieldingItem(wieldedItem, wieldedSlot);
                return;
            }

            DropItemManager.SetWieldingItem(null, InventorySlot.None);
        }
    }
}
