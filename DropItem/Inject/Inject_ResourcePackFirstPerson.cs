using DropItem.Handlers;
using Gear;
using HarmonyLib;

namespace DropItem.Inject
{
    [HarmonyPatch(typeof(ResourcePackFirstPerson), nameof(ResourcePackFirstPerson.UpdateInteraction))]
    internal static class Inject_ResourcePackFirstPerson
    {
        private static bool Prefix()
        {
            bool runOriginal = !DropItemManager.InteractionAllowed;
            return runOriginal;
        }
    }
}
