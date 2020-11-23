using System;
namespace JobsityChatroom.WebAPI.Models.Command
{
    public class Command
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public Command(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
