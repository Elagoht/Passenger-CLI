namespace Passenger
{
  public static class Error
  {
    public static void ArgumentCount(string command, int minOrActual, int max = -1)
    {
      if (minOrActual == 0)
        Console.WriteLine($"passenger: {command}: takes no arguments");
      if (max == -1)
        Console.WriteLine($"passenger: {command}: expected exact {minOrActual} arguments");
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

    public static void ConstantExists(ConstantPair constant)
    {
      Console.WriteLine($"passenger: constant '{constant.Key}' already exists");
      Environment.Exit(1);
    }

    public static void EntryNotFound()
    {
      Console.WriteLine("passenger: entry not found");
      Environment.Exit(1);
    }

    public static void JsonParseError()
    {
      Console.WriteLine("passenger: JSON parse error");
      Environment.Exit(1);
    }

    public static void AskForHelp()
    {
      Console.WriteLine("passenger: try 'passenger --help' for more information");
      Environment.Exit(1);
    }

    public static void FoundOnRepository()
    {
      Console.WriteLine("passenger: your password is on a brute-force repository, not saved");
      Environment.Exit(1);
    }

    public static void FileExceptions(Exception exception)
    {
      switch (exception)
      {
        case FileNotFoundException:
          Console.WriteLine("passenger: data file not found");
          Environment.Exit(1); break;
        case UnauthorizedAccessException:
          Console.WriteLine("passenger: data file access denied");
          Environment.Exit(126); break;
        case IOException:
          Console.WriteLine("passenger: data file input/output error");
          Environment.Exit(1); break;
        default:
          Console.WriteLine("passenger: unexpected error while accessing data file");
          Environment.Exit(2); break;
      }
    }
  }
}