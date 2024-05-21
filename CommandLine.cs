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
        case "login" or "-l": worker.Login(); break;
        case "register" or "-r": worker.Register(); break;
        case "reset" or "-R": worker.Reset(); break;
        case "create" or "-c": worker.Create(); break;
        case "fetchAll" or "-a": worker.FetchAll(); break;
        case "fetch" or "-f": worker.FetchOne(); break;
        case "query" or "-q": worker.Query(); break;
        case "update" or "-u": worker.Update(); break;
        case "delete" or "-d": worker.Delete(); break;
        case "version" or "-v" or "--version": Worker.Version(); break;
        case "help" or "--help" or "-h": Worker.Help(); break;
        case "man" or "-m": Worker.Manual(); break;
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

    public static void InvalidToken()
    {
      Console.WriteLine("passenger: invalid token");
      Environment.Exit(1);
    }

    public static void MissingField(string field)
    {
      Console.WriteLine($"passenger: missing field '{field}'");
      Environment.Exit(1);
    }

    public static void EntryNotFound()
    {
      Console.WriteLine("passenger: entry not found");
      Environment.Exit(1);
    }

    public static void AskForHelp()
    {
      Console.WriteLine("passenger: try 'passenger --help' for more information");
      Environment.Exit(1);
    }
  }
}