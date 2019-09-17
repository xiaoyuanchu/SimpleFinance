using System;
using System.Collections.Generic;
using SimpleFinance.Utility.Console.CmdParser.Impl;
using SimpleFinance.Utility.Console.CmdParser.Error;

namespace SimpleFinance.Utility.Console.CmdParser
{
    public class ParserFactory
    {
        public enum ParserType
        {
            OnelineParser
        }

        private readonly static Dictionary<ParserType, Type> TYPE_REGISTRY = new Dictionary<ParserType, Type>
        {
            {ParserType.OnelineParser, typeof(OnelineParser)}
        };

        public static IParser GetParser(
            ParserType t, bool caseSensitive = false, Dictionary<string, string> options = null)
        {
            if (!TYPE_REGISTRY.ContainsKey(t))
            {
                throw new InvalidCommandException(
                    String.Format("Unsupported parser type '{0}'!", t));
            }

            // Get impl type
            Type implType = TYPE_REGISTRY[t];

            // Create parser instance dynamically
            IParser p;
            try
            {
                p = (IParser)Activator.CreateInstance(implType, caseSensitive, options);
            }
            catch(Exception e)
            {
                throw new InvalidCommandException(
                    String.Format("Failed to create parser instance of type '{0}', and imple {1}!", 
                    t, implType.Name), e);
            }
            return p;
        }
    }
}