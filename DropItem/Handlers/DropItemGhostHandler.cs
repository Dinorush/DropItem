using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace DropItem.Handlers
{
    internal sealed class DropItemGhostHandler : MonoBehaviour
    {
        private GameObject _ItemGhost = null;

        void Awake()
        {
            DropItemManager.OnUpdated += DropItemManager_OnUpdated;
        }

        void OnDestroy()
        {
            DropItemManager.OnUpdated -= DropItemManager_OnUpdated;
            DespawnGhost();
        }

        private void DropItemManager_OnUpdated()
        {
            TryUpdateGhost();
        }

        [HideFromIl2Cpp]
        void TryUpdateGhost()
        {
            if (!DropItemManager.InteractionAllowed)
            {
                DespawnGhost();
                return;
            }

            var itemSlot = DropItemManager.WieldingSlot;
            var dropSlot = DropItemManager.Slot;
            var itemID = DropItemManager.WieldingItem.Get_pItemData().itemID_gearCRC;
            var prefabParts = ItemSpawnManager.m_loadedPrefabsPerItemMode[(int)ItemMode.Pickup][itemID];
            if (prefabParts == null)
                return;

            if (prefabParts.Count < 1)
                return;

            if (prefabParts[0] == null)
                return;

            var baseTransform = dropSlot.GetTransform(itemSlot);

            DespawnGhost();
            _ItemGhost = Instantiate(prefabParts[0], baseTransform.position, baseTransform.rotation);
            foreach (var renderer in _ItemGhost.GetComponentsInChildren<Renderer>())
            {
                foreach (var material in renderer.materials)
                {
                    material.shader = Shader.Find("Transparent/Diffuse");
                    material.color = Color.black.AlphaMultiplied(0.25f);
                }
            }
        }

        [HideFromIl2Cpp]
        void DespawnGhost()
        {
            if (_ItemGhost != null)
            {
                GameObject.Destroy(_ItemGhost);
            }
            _ItemGhost = null;
        }
    }
}
