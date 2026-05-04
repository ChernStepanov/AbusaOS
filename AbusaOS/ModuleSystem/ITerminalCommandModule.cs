using AbusaOS.Utils;

namespace AbusaOS.ModuleSystem
{
    public interface ITerminalCommandModule : IModule
    {
        void RegisterCommands(TerminalCommandRegistry registry);
    }
}
