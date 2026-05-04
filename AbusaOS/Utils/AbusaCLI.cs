using AbusaOS.ModuleSystem;
using AbusaOS.Windows;
using System.Collections.Generic;
using System.Linq;
using Color = System.Drawing.Color;

namespace AbusaOS.Utils
{
    public class CLICommand
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string[] Aliases { get; private set; }

        public CLICommand(string name, string description, string[] aliases)
        {
            Name = name;
            Description = description;
            Aliases = aliases;
        }

        public virtual void Execute(List<string> args, Terminal instance) { }
    }

    public class AbusaCLI
    {
        static readonly ModuleManager Modules = ModuleManager.Shared;

        public static IEnumerable<CLICommand> Commands => Modules.TerminalCommands.Commands;

        public static void RegisterCommand(CLICommand command)
        {
            Modules.TerminalCommands.Register(command);
        }

        public static void RegisterModule(ITerminalCommandModule module)
        {
            Modules.RegisterTerminalCommandModule(module, new KernelApi());
        }

        public static void RegisterManagedModule(IManagedModule module)
        {
            Modules.RegisterManagedModule(module, new KernelApi());
        }

        public static void EnsureInitialized()
        {
            Modules.EnsureInitialized(new KernelApi());
        }

        public static CLICommand FindCommand(string commandName)
        {
            EnsureInitialized();
            return Modules.TerminalCommands.Find(commandName);
        }

        public static void ParseCommand(string command, Terminal instance)
        {
            EnsureInitialized();
            List<string> args = command.Split(' ').Where(arg => !string.IsNullOrEmpty(arg)).ToList();
            if (args.Count == 0)
            {
                instance.print_str("\n");
                return;
            }

            string commandName = args[0].ToLower();
            CLICommand cmd = FindCommand(commandName);
            if (cmd != null)
            {
                cmd.Execute(args, instance);
                return;
            }

            instance.curcol = Color.Red;
            instance.print_str($"[ERR] No such command {args[0]}\n");
            instance.curcol = Color.White;
        }
    }
}
