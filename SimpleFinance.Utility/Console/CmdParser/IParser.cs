namespace SimpleFinance.Utility.Console.CmdParser
{
    public interface IParser
    {
        UserCommand Do(params object[] args);
    }
}
