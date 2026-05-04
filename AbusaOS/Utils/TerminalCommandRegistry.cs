using System.Collections.Generic;
using System.Linq;

namespace AbusaOS.Utils
{
    public class TerminalCommandRegistry
    {
        private readonly List<CLICommand> commands = new();

        public IEnumerable<CLICommand> Commands => commands;

        public void Register(CLICommand command)
        {
            if (command == null)
            {
                return;
            }

            string name = command.Name.ToLower();
            bool exists = commands.Any(existing =>
                existing.Name.ToLower() == name ||
                existing.Aliases.Any(alias => alias.ToLower() == name));

            if (!exists)
            {
                commands.Add(command);
            }
        }

        public CLICommand Find(string commandName)
        {
            string normalized = commandName.ToLower();
            return commands.FirstOrDefault(command =>
                command.Name.ToLower() == normalized ||
                command.Aliases.Any(alias => alias.ToLower() == normalized));
        }
    }
}
