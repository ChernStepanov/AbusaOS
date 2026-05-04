using AbusaOS.ModuleSystem;
using AbusaOS.Utils;

namespace AbusaOS.Modules
{
    internal class BuiltinTerminalCommands : ITerminalCommandModule
    {
        public string Name => "Builtin Terminal Commands";

        public void Initialize(IKernelApi api)
        {
        }

        public void RegisterCommands(TerminalCommandRegistry registry)
        {
            registry.Register(new CLIClearScreen());
            registry.Register(new CLIInfo());
            registry.Register(new CLIEcho());
            registry.Register(new CLIDir());
            registry.Register(new CLICD());
            registry.Register(new CLICat());
            registry.Register(new CLIHelp());
            registry.Register(new CLICallPSOD());
        }
    }
}
