using AbusaOS.ModuleSystem;
using AbusaOS.Utils;
using AbusaOS.Windows;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;

namespace AbusaOS.Modules
{
    internal class ModuleDiagnosticsModule : IManagedModule
    {
        public string Name => Manifest.Name;

        public ModuleManifest Manifest { get; } = new(
            "abusaos.modules.diagnostics",
            "Module Diagnostics",
            "0.1.0",
            "managed-abusaos-1",
            "Abusa OS",
            "Adds terminal commands for inspecting managed modules.",
            new string[] { "terminal-command" });

        public ManagedModuleExports Exports => new(
            registerTerminalCommands: registry => registry.Register(new CLIModules()));

        public void Initialize(IKernelApi api)
        {
            api.Log.Info("Module Diagnostics initialized");
        }
    }

    internal class CLIModules : CLICommand
    {
        public CLIModules() : base("Modules", "Lists loaded managed modules", new string[] { "modules", "mods" }) { }

        public override void Execute(List<string> args, Terminal instance)
        {
            instance.curcol = Color.Green;
            instance.print_str("Loaded managed modules:\n");
            instance.curcol = Color.White;

            foreach (ModuleManifest manifest in ManagedModuleHost.LoadedManifests)
            {
                string provides = manifest.Provides.Length > 0 ? string.Join(", ", manifest.Provides) : "none";
                instance.print_str($"- {manifest.Name} {manifest.Version} [{manifest.AbiVersion}]\n");
                instance.print_str($"  id: {manifest.Id}\n");
                instance.print_str($"  provides: {provides}\n");
            }
        }
    }
}
