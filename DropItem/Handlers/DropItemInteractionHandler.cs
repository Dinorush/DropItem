using BepInEx.IL2CPP.Utils;
using DropItem.Comps;
using Il2CppInterop.Runtime.Attributes;
using LevelGeneration;
using Localization;
using Player;
using SNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DropItem.Handlers
{
    internal sealed class DropItemInteractionHandler : MonoBehaviour
    {
        private bool _DropAllowed = false;
        private bool _HoldingKey = false;

        Coroutine _InteractionRoutine;

        void Awake()
        {
            DropItemManager.OnUpdated += DropItemManager_OnUpdated;
        }

        void OnDestroy()
        {
            DropItemManager.OnUpdated -= DropItemManager_OnUpdated;
        }

        [HideFromIl2Cpp]
        private void DropItemManager_OnUpdated()
        {
            _DropAllowed = DropItemManager.InteractionAllowed;

            if (_DropAllowed)
            {
                SetDropMessage();
            }
            else
            {
                GuiManager.InteractionLayer.InteractPromptVisible = false;
                GuiManager.InteractionLayer.SetInteractPrompt();
            }
        }

        void Update()
        {
            if (_DropAllowed && !_HoldingKey)
            {
                if (InputMapper.GetButtonDownKeyMouseGamepad(InputAction.Use, eFocusState.FPS))
                {
                    StartRoutine();
                    _HoldingKey = true;
                }
            }
            else
            {
                if (InputMapper.GetButtonUpKeyMouseGamepad(InputAction.Use, eFocusState.FPS))
                {
                    StopRoutine();
                    _HoldingKey = false;
                }
            }
        }

        [HideFromIl2Cpp]
        static void SetDropMessage()
        {
            GuiManager.InteractionLayer.InteractPromptVisible = true;
            var message = string.Format(Text.Get(864), DropItemManager.WieldingItem.PublicName); //InGame.InteractionPrompt.Drop_X
            var button = string.Format(Text.Get(827), InputMapper.GetBindingName(InputAction.Use)); //InGame.InteractionPrompt.Hold_X
            GuiManager.InteractionLayer.SetInteractPrompt(message, button);
        }

        [HideFromIl2Cpp]
        void StartRoutine()
        {
            StopRoutine();
            _InteractionRoutine = this.StartCoroutine(Interaction(0.4f));
            StartInteraction();
        }

        [HideFromIl2Cpp]
        void StopRoutine()
        {
            if (_InteractionRoutine != null)
            {
                StopCoroutine(_InteractionRoutine);
                StopInteraction();
            }
        }

        [HideFromIl2Cpp]
        private static void StartInteraction()
        {
            GuiManager.InteractionLayer.InteractPromptVisible = true;
        }

        [HideFromIl2Cpp]
        private static void StopInteraction()
        {
            GuiManager.InteractionLayer.InteractPromptVisible = false;
        }

        [HideFromIl2Cpp]
        IEnumerator Interaction(float interactionTime)
        {
            SetDropMessage();

            var timer = 0.0f;
            var timerInterrupted = false;
            while(timer <= interactionTime)
            {
                if (!DropItemManager.InteractionAllowed)
                {
                    timerInterrupted = true;
                    break;
                }
                GuiManager.InteractionLayer.SetTimer(timer / interactionTime);
                timer += Time.deltaTime;
                yield return null;
            }

            if (!timerInterrupted)
            {
                DropItemManager.DropWieldingItemToSlot();
            }

            StopInteraction();
            _InteractionRoutine = null;
        }
    }
}
