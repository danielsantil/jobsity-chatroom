using System;
using JobsityChatroom.WebAPI.Models.Command;
using JobsityChatroom.WebAPI.Services;
using Xunit;

namespace JobsityChatroom.UnitTest
{
    public class ChatCommandTest
    {
        private readonly IChatCommandService commandService;

        public ChatCommandTest()
        {
            commandService = new ChatCommandService();
        }

        [Fact]
        public void ValidCommand()
        {
            var result = commandService.IsCommand("/stock=aapl.us");
            Assert.True(result);
        }

        [Fact]
        public void NotACommand()
        {
            var result = commandService.IsCommand("stock=aapl.us");
            Assert.False(result);
        }

        [Fact]
        public void CommandExecutes_NoExceptions()
        {
            void call() => commandService.Handle("/stock=aapl.us", _ => { });
            var exception = Record.Exception(call);

            Assert.Null(exception);
        }

        [Fact]
        public void InvalidSyntax()
        {
            void call() => commandService.Handle("/stock/stock", _ => { });
            var exception = Record.Exception(call);

            Assert.NotNull(exception);
            Assert.IsType<CommandException>(exception);
            Assert.Equal("Invalid command syntax", exception.Message);
        }

        [Fact]
        public void UnknownCommand()
        {
            void call() => commandService.Handle("/stocc=aapl.us", _ => { });
            var exception = Record.Exception(call);

            Assert.NotNull(exception);
            Assert.IsType<CommandException>(exception);
            Assert.Equal("Unknown command", exception.Message);
        }
    }
}
