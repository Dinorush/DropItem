using DropItem.Handlers;
using HarmonyLib;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropItem.Inject
{
    [HarmonyPatch(typeof(LocalPlayerAgent), nameof(LocalPlayerAgent.Setup))]
    internal static class Inject_LocalPlayerAgent
    {
        private static void Postfix(LocalPlayerAgent __instance)
        {
            var obj = __instance.gameObject;
            if (obj.GetComponent<DropItemUpdater> == null)
            {
                var handler = obj.AddComponent<DropItemUpdater>();
                obj.AddComponent<DropItemGhostHandler>();
                obj.AddComponent<DropItemInteractionHandler>();
                handler.Agent = __instance;
            }
        }
    }
}
