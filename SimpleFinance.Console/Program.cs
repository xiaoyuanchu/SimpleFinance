using System;
using SysConsole = System.Console;
using SimpleFinance.Utility.Console.CmdParser;

namespace SimpleFinance.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            SysConsole.WriteLine("Hello World!");


            string line =
            "  cmd1-test ex1 -p1 -p2=abc  ex2  -p3:true -p4=\"hello\" /p5 /p6=\"split-values\\ here\" -p7:123 \"ex3 more\" ";
            IParser p = ParserFactory.GetParser(
                ParserFactory.ParserType.OnelineParser);
            UserCommand c = p.Do(line);
            SysConsole.WriteLine(c.ToString());
        }
    }
}
