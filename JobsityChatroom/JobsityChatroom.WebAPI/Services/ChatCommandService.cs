using System;
using System.Collections.Generic;
using JobsityChatroom.WebAPI.Models.Command;

namespace JobsityChatroom.WebAPI.Services
{
    public class ChatCommandService : IChatCommandService
    {
        private readonly List<string> availableCommands;

        public ChatCommandService()
        {
            availableCommands = new List<string> { "stock" };
        }

        public void Handle(string message, Action<Command> action)
        {
            var command = CreateCommand(message);
            action(command);
        }

        public bool IsCommand(string message)
        {
            return !string.IsNullOrWhiteSpace(message) && message.StartsWith("/");
        }

        private Command CreateCommand(string message)
        {
            if (!IsCommand(message))
                throw new CommandException("Message is not a command");

            var parts = message.ToLower().Substring(1).Split("=");
            if (parts.Length != 2)
                throw new CommandException("Invalid command syntax");

            if (!availableCommands.Contains(parts[0]))
                throw new CommandException("Unknown command");

            return new Command(parts[0], parts[1]);
        }
    }
}
