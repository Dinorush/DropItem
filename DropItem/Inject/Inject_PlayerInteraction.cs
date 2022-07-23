using DropItem.Handlers;
using HarmonyLib;

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
