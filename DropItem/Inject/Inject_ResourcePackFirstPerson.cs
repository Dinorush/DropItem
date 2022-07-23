using DropItem.Handlers;
using Gear;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
