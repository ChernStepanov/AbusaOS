using AbusaOS.Modules;
using AbusaOS.Utils;
using System.Collections.Generic;

namespace AbusaOS.ModuleSystem
{
    public class ModuleManager
    {
        static readonly ModuleManager shared = new();
        readonly ModuleRegistry applications = new();
        readonly TerminalCommandRegistry terminalCommands = new();
        readonly List<IModule> loadedModules = new();
        bool initialized;

        public static ModuleManager Shared => shared;
        public ModuleRegistry Applications => applications;
        public TerminalCommandRegistry TerminalCommands => terminalCommands;
        public IEnumerable<IModule> LoadedModules => loadedModules;

        public void EnsureInitialized(IKernelApi api)
        {
            if (initialized)
            {
                return;
            }

            RegisterBuiltinModules(api);
            initialized = true;
        }

        public void RegisterApplicationModule(IApplicationModule module, IKernelApi api)
        {
            if (module == null)
            {
                return;
            }

            module.Initialize(api);
            applications.Register(module);
            loadedModules.Add(module);
        }

        public void RegisterTerminalCommandModule(ITerminalCommandModule module, IKernelApi api)
        {
            if (module == null)
            {
                return;
            }

            module.Initialize(api);
            module.RegisterCommands(terminalCommands);
            loadedModules.Add(module);
        }

        public void RegisterManagedModule(IManagedModule module, IKernelApi api)
        {
            if (ManagedModuleHost.Register(module, api, applications, terminalCommands))
            {
                loadedModules.Add(module);
            }
        }

        void RegisterBuiltinModules(IKernelApi api)
        {
            BuiltinModules.Register(this, api);
            RegisterTerminalCommandModule(new BuiltinTerminalCommands(), api);
            RegisterManagedModule(new ModuleDiagnosticsModule(), api);
        }
    }
}
