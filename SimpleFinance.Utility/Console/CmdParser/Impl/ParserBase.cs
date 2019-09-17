using System;
using System.Collections.Generic;

namespace SimpleFinance.Utility.Console.CmdParser.Impl
{
    abstract class ParserBase : IParser
    {   
        protected static char[] KEYWORD_PARAM_NAME = {'-', '/'};
        protected static char[] KEYWORD_VALUE_ASSIGNMENT = {'=', ':'};
        protected static char[] KEYWORD_QUOTES = {'"', '\''};
        
        public ParserBase(bool caseSensitive, Dictionary<string, string> options)
        {
            this.CaseSensitive = caseSensitive;
            this.options = options ?? new Dictionary<string, string>();
        }

        public bool CaseSensitive { get; set; }

        private Dictionary<string, string> options;
        protected Dictionary<string, string> Options
        {
            get { return this.options; }
        }

        protected bool ContainsOption(string key)
        {
            if (null == key)
                return false;
            return this.Options.ContainsKey(key);
        }

        protected string GetOption(string key)
        {
            if (this.ContainsOption(key))
                return this.Options[key];
            throw new InvalidCastException(
                String.Format("Option key {0} not found in argumens!", key));
        }

        public UserCommand Do(params object[] args)
        {
            return Parse(args);
        }

        protected abstract UserCommand Parse(object[] args);
    }
}