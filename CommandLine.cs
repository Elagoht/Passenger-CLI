/// <summary>
/// This class contains the command line interface.
/// </summary>
/// <remarks>
/// This class contains the command line interface.
/// </remarks>
namespace Passenger
{
  /// <summary>
  /// Command line interface
  /// </summary>
  /// <param name="args"></param>
  /// <remarks>
  /// Command line verb and argument parser.
  /// </remarks>
  public class CommandLine(string[] args)
  {
    private readonly string command = args[0];

    /// <summary>
    /// Parse command line arguments
    /// </summary>
    /// <remarks>
    /// Determine the command and call the appropriate method.
    /// </remarks>
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
        case "fetch" or "-f": worker.Fetch(); break;
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

  /// <summary>
  /// Error messages
  /// </summary>
  /// <remarks>
  /// Produce error messages and exits with appropriate status code.
  /// </remarks>
  public static class Error
  {
    /// <summary>
    /// Argument count error
    /// </summary>
    /// <param name="command"></param>
    /// <param name="minOrActual"></param>
    /// <param name="max"></param>
    /// <remarks>
    /// Prints the required number of arguments for a command.
    /// </remarks>
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

    /// <summary>
    /// Missing command
    /// </summary>
    /// <remarks>
    /// Verb is not provided.
    /// </remarks>
    public static void MissingCommand()
    {
      Console.WriteLine("passenger: missing command");
      Environment.Exit(1);
    }

    /// <summary>
    /// Unknown command
    /// </summary>
    /// <remarks>
    /// A command that is not recognized by the program is provided.
    /// </remarks>
    public static void UnknownCommand()
    {
      Console.WriteLine("passenger: unknown command");
      Environment.Exit(1);
    }

    /// <summary>
    /// Invalid token
    /// </summary>
    /// <remarks>
    /// Authorization could not be verified.
    /// </remarks>
    public static void InvalidToken()
    {
      Console.WriteLine("passenger: invalid token");
      Environment.Exit(1);
    }

    /// <summary>
    /// Missing field
    /// </summary>
    /// <param name="field"></param>
    /// <remarks>
    /// A required field of a Passphrase entry is missing.
    /// </remarks>
    public static void MissingField(string field)
    {
      Console.WriteLine($"passenger: missing field '{field}'");
      Environment.Exit(1);
    }

    /// <summary>
    /// Entry not found
    /// </summary>
    /// <remarks>
    /// Passphrase entry trying to be accessed is not found.
    /// </remarks>
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

    /// <summary>
    /// Ask for help
    /// </summary>
    /// <remarks>
    /// Suggests the user to use the help command.
    /// </remarks>
    public static void AskForHelp()
    {
      Console.WriteLine("passenger: try 'passenger --help' for more information");
      Environment.Exit(1);
    }
  }
}