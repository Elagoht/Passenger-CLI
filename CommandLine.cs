namespace Passenger
{
  public class CommandLine(string[] args)
  {
    private readonly string command = args[0];
    public void Parse()
    {
      if (string.IsNullOrEmpty(command)) Error.MissingCommand();
      Worker worker = new(args);
      switch (command)
      {
        case "help" or "--help" or "-h": Worker.Help(); break;
        case "login" or "-l": worker.Login(); break;
        case "register" or "-r": worker.Register(); break;
        case "reset" or "-R": worker.Reset(); break;
        case "get" or "-g": worker.Get(); break;
        case "query" or "-q": worker.Query(); break;
        case "save" or "-s": worker.Save(); break;
        default: Error.UnknownCommand(); break;
      }
    }
  }

  public static class Error
  {
    public static void ArgumentCount(string command, int minOrActual, int max = -1)
    {
      if (minOrActual == 0)
        Console.WriteLine($"passenger: {command}: takes no arguments");
      if (max == -1)
        Console.WriteLine($"passenger: {command}: expected exactly {minOrActual} arguments");
      else
        Console.WriteLine($"passenger: {command}: expected {minOrActual}-{max} arguments");
      AskForHelp();
      Environment.Exit(2);
    }
    public static void MissingCommand()
    {
      Console.WriteLine("passenger: missing command");
      Environment.Exit(1);
    }
    public static void UnknownCommand()
    {
      Console.WriteLine("passenger: unknown command");
      Environment.Exit(1);
    }
    public static void AskForHelp()
    {
      Console.WriteLine("passenger: try 'passenger --help' for more information");
      Environment.Exit(1);
    }
  }
}