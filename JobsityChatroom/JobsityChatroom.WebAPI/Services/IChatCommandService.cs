using System;
using JobsityChatroom.WebAPI.Models.Command;

namespace JobsityChatroom.WebAPI.Services
{
    public interface IChatCommandService
    {
        void Handle(string message, Action<Command> action);
        bool IsCommand(string message);
    }
}