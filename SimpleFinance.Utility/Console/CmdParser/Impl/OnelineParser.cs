using System;
using System.Collections.Generic;
using SimpleFinance.Utility.Console.CmdParser.Error;

namespace SimpleFinance.Utility.Console.CmdParser.Impl
{
    /// Supported Formmat:
    /// command name: comes first in the line, and start with letter or digit
    /// parameters:
    ///    parameter name starts with "-" or "/", name starts with letter or digit
    ///    (optional, null if not exist) parameter value starts with "=" or ":", 
    ///         white spaces escaped enless enclosed in ""
    /// extras: starts with letters
    /// e.g.
    ///    cmd1-test ex1 -p1 -p2=abc  ex2  -p3:true -p4=\"hello\" /p5 /p6=\"split-values\\ here\" -p7:123 \"ex3 more\" 
    class OnelineParser : ParserBase
    {
        public OnelineParser(bool casSensitive, Dictionary<string, string> options)
            : base(casSensitive, options) {}

        override
        protected UserCommand Parse(object[] args)
        {
            if (args == null || args.Length <= 0)
                throw new InvalidCommandException("Empty content input!");
            
            string line = args[0] as string;
            if (string.IsNullOrWhiteSpace(line))
                throw new InvalidCommandException("Empty content input!");

            // Start parsing process
            char[] raw = line.ToCharArray();
            int begin = 0;
            int end = raw.Length; // raw[begin:end)
            
            // Get command name
            GetCommandName(raw, ref begin, ref end);
            string commandName = line.Substring(begin, end-begin);
            begin = end;
            UserCommand c = new UserCommand(commandName, this.CaseSensitive);
            string paramName = null;
            string paramValue = null;
            string extra = null;

            // Parse parameters and extras
            while (begin < raw.Length)
            {
                // Skipping white spaces
                SkipWhitespaces(raw, ref begin, ref end);
                if (begin >= raw.Length)
                    break; // no spaces or extras

                // Test if is parameter or extra
                bool quoted = false;
                if ( IsKeywordParamName(raw[begin]) )
                {
                    // Get parameter name
                    bool hasValue = GetParameterName(raw, ref begin, ref end);
                    paramName = line.Substring(begin, end-begin);
                    begin = end;
                    
                    // Get parameter value
                    if (hasValue)
                    {
                        quoted = GetParameterValue(raw, ref begin, ref end);
                        paramValue = line.Substring(begin, end-begin);
                        begin = end;
                    }
                    else
                    {
                        paramValue = null;
                    }

                    // Add to command
                    c.AddParam(paramName, paramValue);
                }
                else
                {
                    // Get Extra value
                    quoted = GetExtra(raw, ref begin, ref end);
                    extra = line.Substring(begin, end-begin);
                    begin = end;

                    // Add to command
                    c.AddExtra(extra);
                }

                // Escape tailing " or ' if exists
                if (quoted)
                    begin++;
            }

            return c;
        }

        private void SkipWhitespaces(char[] raw, ref int begin, ref int end)
        {
            while(begin < raw.Length)
            {
                if (Char.IsWhiteSpace(raw[begin]))
                    begin++;
                else
                    break;
            }
            end = begin + 1;
        }

        private void GetValidName(char[] raw, ref int begin, ref int end, Func<char, bool> isEnd)
        {
            if (begin >= raw.Length)
                throw new InvalidCommandException(
                    String.Format("Invalid command! valid name not found! {0}", raw.ToString())
                );

            // Valid name starts with letter or digit
            if ( ! char.IsLetterOrDigit(raw[begin]))
                throw new InvalidCommandException(
                    String.Format("Invalid name: '{0}'! name/value should be started with letter or digit!", 
                    raw.ToString())
                );
            end = begin + 1;

            // Get name: [begin, end) whitespaces
            while (end < raw.Length)
            {
                if ( isEnd(raw[end]) )
                    break;
                end++;
            }
        }

        // Get valid name (not including name in "" or '')
        private void GetValidName(char[] raw, ref int begin, ref int end)
        {
            // Default ended with whitespaces
            GetValidName(raw, ref begin, ref end, c => char.IsWhiteSpace(c));
        }
        
        // Get valid quoted name (included in "" or '')
        private void GetValidQuotedName(char[] raw, ref int begin, ref int end)
        {
            if (begin >= raw.Length)
                throw new InvalidCommandException(
                    String.Format("Invalid command! valid name not found! {0}", raw.ToString())
                );

            // Must starts with " or '
            char quotesSymbol = raw[begin];
            if ( ! IsKeywordQuotes(quotesSymbol) )
                throw new InvalidCommandException(
                    String.Format("\" or ' expected in command! {0}", raw.ToString())
                );

            // Skip the starting " or '
            begin++;
            end = begin;

            // Read until quotes marks matched
            bool matched = false;
            while (end < raw.Length)
            {
                if (quotesSymbol == raw[end])
                {
                    matched = true;
                    break;
                }
                end++;
            }

            if ( ! matched )
                throw new InvalidCommandException(
                    String.Format("Invalid name/value in command! Should be enclosed in matched quote marks! {0}", raw.ToString())
                );
        }

        private void GetCommandName(char[] raw, ref int begin, ref int end)
        {
            // Skip whitespaces
            SkipWhitespaces(raw, ref begin, ref end);

            // Get valid name
            GetValidName(raw, ref begin, ref end);
        }

        private bool IsKeywordOf(char c, char[] keywords)
        {
            foreach (char key in keywords)
            {
                if (c == key)
                    return true;
            }
            return false;
        }

        private bool IsKeywordParamName(char c)
        {
            return IsKeywordOf(c, KEYWORD_PARAM_NAME);
        }

        private bool IsKeywordQuotes(char c)
        {
            return IsKeywordOf(c, KEYWORD_QUOTES);
        }

        private bool IsKeywordAssignment(char c)
        {
            return IsKeywordOf(c, KEYWORD_VALUE_ASSIGNMENT);
        }

        // return true if enclosed in "" or ''; false otherwise
        private bool GetExtra(char[] raw, ref int begin, ref int end)
        {
            // Skip whitespaces if necessary
            SkipWhitespaces(raw, ref begin, ref end);
            if (begin >= raw.Length)
                throw new InvalidCommandException(
                    String.Format("Invalid command! valid name not found! {0}", raw.ToString())
                );

            // Extras enclosed in ""/'' or straightly starts with the contents (no whitespace within)
            // If within ""/'' marks
            char c = raw[begin];
            bool quoted = IsKeywordQuotes(c);
            if (quoted)
            {
                GetValidQuotedName(raw, ref begin, ref end);
            }
            else
            {
                // Get valid name
                GetValidName(raw, ref begin, ref end);
            }
            return quoted;
        }

        // true if value is following; other wise false;
        private bool GetParameterName(char[] raw, ref int begin, ref int end)
        {
            // Skip whitespaces if necessary
            SkipWhitespaces(raw, ref begin, ref end);
            if (begin >= raw.Length)
                throw new InvalidCommandException(
                    String.Format("Invalid command! valid parameter name not found! {0}", raw.ToString())
                );

            // Skip initial - or /
            begin ++;

            // Get valid parameter name
            GetValidName(raw, ref begin, ref end, 
                c => (char.IsWhiteSpace(c)) || (IsKeywordAssignment(c))
                );

            if ( (end < raw.Length) && IsKeywordAssignment(raw[end]) )
                return true; // Has value
            return false;
        }

        // return true if enclosed in "" or ''; false otherwise
        private bool GetParameterValue(char[] raw, ref int begin, ref int end)
        {
            // Skip initial = or :
            begin ++;
            if (begin >= raw.Length)
                throw new InvalidCommandException(
                    String.Format("Invalid command! valid parameter value not found! {0}", raw.ToString())
                );

            // Enclosed in ""/'' or straightly starts with the contents (no whitespace within)
            // If within ""/'' marks
            char c = raw[begin];
            bool quoted = IsKeywordQuotes(c);
            if (quoted)
            {
                GetValidQuotedName(raw, ref begin, ref end);
            }
            else
            {
                // Get valid name
                GetValidName(raw, ref begin, ref end);
            }
            return quoted;
        }


    }
}
