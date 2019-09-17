using System;

namespace SimpleFinance.Utility.Console.CmdParser.Error
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException() : base() {}

        public InvalidCommandException(string message) : base(message) {}

        public InvalidCommandException(string message, Exception e) : base(message, e) {}
    }
}