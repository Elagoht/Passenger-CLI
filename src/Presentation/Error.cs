using System.Security.Cryptography;

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

    public static void SecretKeyNotProvided()
    {
      Console.WriteLine("passenger: secret key not provided");
      Environment.Exit(1);
    }

    public static void DatabaseLoadFailed()
    {
      Console.WriteLine("passenger: failed to load or decrypt database");
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

    public static void PassphraseTooShort()
    {
      Console.WriteLine("passenger: passphrase must be at least 8 characters long");
      Environment.Exit(1);
    }

    public static void PassphraseTooLong()
    {
      Console.WriteLine("passenger: more than 4096 characters passphrases are not supported");
      Environment.Exit(1);
    }

    public static void BrowserTypeNotSupported()
    {
      Console.WriteLine("passenger: browser type not supported");
      Environment.Exit(1);
    }

    public static void ExportTypeNotSupported()
    {
      Console.WriteLine("passenger: export type not supported");
      Environment.Exit(1);
    }

    public static void ImportFailed()
    {
      Console.WriteLine("passenger: failed to import data");
      Environment.Exit(1);
    }

    public static void ImportSkippedEntries(int count)
    {
      Console.WriteLine($"passenger: {(count > 1
        ? $"{count} entries"
        : "1 entry"
      )} entries skipped due to short password");
      Environment.Exit(1);
    }

    public static void ImportDenied(int count)
    {
      Console.WriteLine($"passenger: import denied due to {(count > 1
        ? $"{count} entries"
        : "1 entry"
      )} with short password");
      Environment.Exit(1);
    }

    public static void PipedInputRequired()
    {
      Console.WriteLine("passenger: Input not provided");
      Environment.Exit(1);
    }

    public static void CSVFormatMissmatch()
    {
      Console.WriteLine("passenger: CSV format mismatched with the specified browser");
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
        case AuthenticationTagMismatchException:
          Console.WriteLine("passenger: Authentication cannot be verified");
          Environment.Exit(1); break;
        default:
          Console.WriteLine("passenger: unexpected error while accessing data file");
          Environment.Exit(2); break;
      }
    }
  }
}