using BepInEx;
using BepInEx.IL2CPP;
using DropItem.Comps;
using DropItem.Handlers;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;

namespace DropItem
{
    [BepInPlugin("DropItem.GUID", VersionInfo.RootNamespace, VersionInfo.Version)]
    [BepInDependency("dev.gtfomodding.gtfo-api", BepInDependency.DependencyFlags.HardDependency)]
    internal class EntryPoint : BasePlugin
    {
        private Harmony _Harmony;

        public override void Load()
        {
            ClassInjector.RegisterTypeInIl2Cpp<DropItemUpdater>();
            ClassInjector.RegisterTypeInIl2Cpp<DropItemInteractionHandler>();
            ClassInjector.RegisterTypeInIl2Cpp<DropItemGhostHandler>();
            ClassInjector.RegisterTypeInIl2Cpp<ResourceContainerSlot>();

            _Harmony = new Harmony($"{VersionInfo.RootNamespace}.Harmony");
            _Harmony.PatchAll();
        }
    }
}