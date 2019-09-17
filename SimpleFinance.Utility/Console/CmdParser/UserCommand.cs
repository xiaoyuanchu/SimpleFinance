using System;
using System.Text;
using System.Collections.Generic;
using SimpleFinance.Utility.Console.CmdParser.Error;

namespace SimpleFinance.Utility.Console.CmdParser
{
    /// Command:
    /// name, {(param1: value1)， (param2: value2)， ...}, {extra1, extra2, ...}
    public class UserCommand
    {
        /// Constructor
        public UserCommand(string name, bool caseSensitive = false)
        {
            this.CaseSensitive = caseSensitive;
            this.Name = Unify(name);

            if (String.IsNullOrWhiteSpace(this.Name))
            {
                throw new CommandNotFoundException(
                    "Failed to create UserCommand instance! Empty/null command name.");
            }
        }

        /// Return string with uniformed case
        protected string Unify(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return null;
            
            if (this.CaseSensitive)
                return s.Trim();
            else
                return s.Trim().ToLower();  // Use lower case.
        }

        private bool caseSensitive;
        public bool CaseSensitive 
        { 
            get { return this.caseSensitive; }
            protected set { this.caseSensitive = value; }
        }

        private string name;
        public string Name 
        {
            get { return this.name; }
            protected set { this.name = value; }
        }

        private HashSet<string> extras = new HashSet<string>();
        public ICollection<string> Extras
        {
            get { return this.extras; }
        }

        // Ignore null or empty strings
        // false if not added
        public bool AddExtra(string ex)
        {
            ex = Unify(ex);
            if (null == ex)
                return false;
            return this.extras.Add(ex);
        }

        public bool RemoveExtra(string ex)
        {
            ex = Unify(ex);
            if (null == ex)
                return false;
            return this.extras.Remove(ex);
        }

        public bool ContainsExtra(string ex)
        {
            ex = Unify(ex);
            if (null == ex)
                return false;
            return this.extras.Contains(ex);
        } 
        
        public class Param
        {
            public Param() {}
            public Param(string name, string value) 
            {
                Name = name;
                Value = value;
            }
            public string Name;
            public string Value;       
        }

        // Dictionary<Parameter name: Param>
        private Dictionary<string, Param> parameters = new Dictionary<string, Param>();
        public ICollection<Param> Params 
        {
            get{ return this.parameters.Values; }
        }

        public void AddParam(string name, string value)
        {
            AddParam(new Param(name, value));
        }

        // false if not added
        public bool AddParam(Param p)
        {
            if (null == p)
                return false;
            
            // Unify case for parameter name
            p.Name = Unify(p.Name);
            if (null == p.Name)
                return false;
            
            // Set value null for whitespaces
            if (string.IsNullOrWhiteSpace(p.Value))
                p.Value = null;

            if (this.parameters.ContainsKey(p.Name))
            {
                // Replace duplicated name value
                this.parameters[p.Name] = p;
            }
            else
            {
                this.parameters.Add(p.Name, p);
            }
            return true;
        }

        public bool RemoveParam(string paramName)
        {
            paramName = Unify(paramName);
            if (null == paramName)
                return false;
            
            return this.parameters.Remove(paramName);
        }

        public bool ContainsParam(string paramName)
        {
            paramName = Unify(paramName);
            if (null == paramName)
                return false;

            return this.parameters.ContainsKey(paramName);
        }

        // Get param value
        public string this[string paramName]
        {
            get 
            {
                if (this.ContainsParam(paramName))
                {
                    // Parameter value (can be null)
                    return this.parameters[Unify(paramName)].Value;
                }
                else
                {
                    // Not found
                    throw new CommandNotFoundException(
                        String.Format("Parameter name '{0}' not found!", paramName));
                }
            }
            set
            {
                // Set parameter
                this.AddParam(paramName, value);
            }
        }

        override
        public string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{ ").Append(Name).Append(", { ");
            foreach (Param p in Params)
            {
                sb.Append(p.Name).Append(":").Append(p.Value).Append(", ");
            }
            sb.Append("}, ").Append("{ ");
            foreach (string e in Extras)
            {
                sb.Append(e).Append(", ");
            }
            return sb.Append("} }").ToString();
        }
        
    }
}