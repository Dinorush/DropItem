using DropItem.Handlers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropItem.Inject
{
    [HarmonyPatch(typeof(PlayerInteraction), nameof(PlayerInteraction.UpdateWorldInteractions))]
    internal static class Inject_PlayerInteraction
    {
        private static bool Prefix()
        {
            bool runOriginal = !DropItemManager.InteractionAllowed;
            return runOriginal;
        }
    }
}
