using AbusaOS.Utils;
using System;
using System.Collections.Generic;

namespace AbusaOS.ModuleSystem
{
    public class ModuleManifest
    {
        public string Id { get; }
        public string Name { get; }
        public string Version { get; }
        public string AbiVersion { get; }
        public string Author { get; }
        public string Description { get; }
        public string[] Provides { get; }

        public ModuleManifest(string id, string name, string version, string abiVersion, string author, string description, string[] provides)
        {
            Id = id;
            Name = name;
            Version = version;
            AbiVersion = abiVersion;
            Author = author;
            Description = description;
            Provides = provides ?? Array.Empty<string>();
        }
    }

    public class ManagedModuleExports
    {
        public Action<IKernelApi> Initialize { get; }
        public Action<ModuleRegistry> RegisterApplications { get; }
        public Action<TerminalCommandRegistry> RegisterTerminalCommands { get; }

        public ManagedModuleExports(
            Action<IKernelApi> initialize = null,
            Action<ModuleRegistry> registerApplications = null,
            Action<TerminalCommandRegistry> registerTerminalCommands = null)
        {
            Initialize = initialize;
            RegisterApplications = registerApplications;
            RegisterTerminalCommands = registerTerminalCommands;
        }
    }

    public interface IManagedModule : IModule
    {
        ModuleManifest Manifest { get; }
        ManagedModuleExports Exports { get; }
    }

    public static class ManagedModuleHost
    {
        const string CurrentAbiVersion = "managed-abusaos-1";
        static readonly List<ModuleManifest> loadedManifests = new();

        public static IEnumerable<ModuleManifest> LoadedManifests => loadedManifests;

        public static bool Register(IManagedModule module, IKernelApi api, ModuleRegistry applicationRegistry = null, TerminalCommandRegistry commandRegistry = null)
        {
            if (module == null || module.Manifest == null)
            {
                return false;
            }

            if (module.Manifest.AbiVersion != CurrentAbiVersion)
            {
                api.Log.Warning("Skipped module with incompatible ABI: " + module.Manifest.Name);
                return false;
            }

            module.Initialize(api);
            module.Exports?.Initialize?.Invoke(api);
            if (applicationRegistry != null)
            {
                module.Exports?.RegisterApplications?.Invoke(applicationRegistry);
            }
            if (commandRegistry != null)
            {
                module.Exports?.RegisterTerminalCommands?.Invoke(commandRegistry);
            }
            loadedManifests.Add(module.Manifest);
            return true;
        }
    }
}
