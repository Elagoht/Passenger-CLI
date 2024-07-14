using System.Security.Cryptography;

namespace Passenger
{
  public static class Error
  {
    public static void ArgumentCount(string command, int minOrActual, int max = -1)
    {
      if (minOrActual == 2)
        Console.Error.WriteLine($"passenger: {command}: takes no arguments");
      if (max == -1)
        Console.Error.WriteLine($"passenger: {command}: expected exact {minOrActual} arguments");
      else
        Console.Error.WriteLine($"passenger: {command}: expected {minOrActual}-{max} arguments");
      AskForHelp();
      Environment.Exit(40);
    }

    public static void MissingCommand()
    {
      Console.Error.WriteLine("passenger: missing command");
      Environment.Exit(40);
    }

    public static void UnknownCommand()
    {
      Console.Error.WriteLine("passenger: unknown command");
      Environment.Exit(40);
    }

    public static void InvalidToken()
    {
      Console.Error.WriteLine("passenger: invalid token");
      Environment.Exit(41);
    }

    public static void InvalidPassphrase()
    {
      Console.Error.WriteLine("passenger: master passphrase did not match");
      Environment.Exit(40);
    }

    public static void SecretKeyNotProvided()
    {
      Console.Error.WriteLine("passenger: secret key not provided");
      Environment.Exit(41);
    }

    public static void DatabaseLoadFailed()
    {
      Console.Error.WriteLine("passenger: failed to load or decrypt database");
      Environment.Exit(50);
    }

    public static void MissingField(string field)
    {
      Console.Error.WriteLine($"passenger: missing field '{field}'");
      Environment.Exit(40);
    }

    public static void ConstantExists(ConstantPair constant)
    {
      Console.Error.WriteLine($"passenger: constant '{constant.Key}' already exists");
      Environment.Exit(49);
    }

    public static void EntryNotFound()
    {
      Console.Error.WriteLine("passenger: entry not found");
      Environment.Exit(44);
    }

    public static void JsonParseError()
    {
      Console.Error.WriteLine("passenger: JSON parse error");
      Environment.Exit(46);
    }

    public static void AskForHelp()
    {
      Console.Error.WriteLine("passenger: try 'passenger --help' for more information");
      Environment.Exit(33);
    }

    public static void FoundOnRepository()
    {
      Console.Error.WriteLine("passenger: your password is on a brute-force repository, not saved");
      Environment.Exit(43);
    }

    public static void PassphraseTooShort()
    {
      Console.Error.WriteLine("passenger: passphrase must be at least 8 characters long");
      Environment.Exit(46);
    }

    public static void PassphraseTooLong()
    {
      Console.Error.WriteLine("passenger: more than 4096 characters passphrases are not supported");
      Environment.Exit(46);
    }

    public static void BrowserTypeNotSupported()
    {
      Console.Error.WriteLine("passenger: browser type not supported");
      Environment.Exit(45);
    }

    public static void ExportTypeNotSupported()
    {
      Console.Error.WriteLine("passenger: export type not supported");
      Environment.Exit(45);
    }

    public static void ImportFailed()
    {
      Console.Error.WriteLine("passenger: failed to import data");
      Environment.Exit(50);
    }

    public static void ImportHasBadEntries(List<DatabaseEntry> skippedEntries, List<DatabaseEntry> mappedEntries)
    {
      /**
       * User can pipe the stdout to a file to import
       * or stderr to see the skipped entries to fix them.
       */
      Console.Error.WriteLine(
        "Skipped entries:\nname,url,username,password,note\n" +
        string.Join('\n', skippedEntries.Select(Mapper.ToCSVLine)) +
        "\nOther entries are acceptable.\n"
      );
      Console.WriteLine(
        "name,url,username,password,note\n" +
        string.Join('\n', mappedEntries.Select(Mapper.ToCSVLine))
      );
      Environment.Exit(40);
    }

    public static void PipedInputRequired()
    {
      Console.Error.WriteLine("passenger: Input not provided");
      Environment.Exit(40);
    }

    public static void CSVFormatMissmatch()
    {
      Console.Error.WriteLine("passenger: CSV format mismatched with the specified browser");
      Environment.Exit(40);
    }

    public static void FileExceptions(Exception exception)
    {
      switch (exception)
      {
        case FileNotFoundException:
          Console.Error.WriteLine("passenger: data file not found");
          Environment.Exit(50); break;
        case UnauthorizedAccessException:
          Console.Error.WriteLine("passenger: data file access denied");
          Environment.Exit(43); break;
        case IOException:
          Console.Error.WriteLine("passenger: data file input/output error");
          Environment.Exit(50); break;
        case AuthenticationTagMismatchException:
          Console.Error.WriteLine("passenger: Authentication cannot be verified");
          Environment.Exit(41); break;
        default:
          Console.Error.WriteLine("passenger: unexpected error while accessing data file");
          Environment.Exit(50); break;
      }
    }
  }
}