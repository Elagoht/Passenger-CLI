namespace Passenger
{
  public class CommandLine(string[] args)
  {
    private readonly Authorization authorization = new(EnDeCoder.JSWSecret);
    private readonly string command = args[0];
    private readonly string[] arguments = args.Skip(1).ToArray();
    private readonly string helpText = @"Passenger CLI 0.1.0
  Copyright (C) 2024 Elagoht
  
  Store, retrieve, manage and generate passphrases securely using your own encode/decode algorithm. Every passenger client is created by user's itself and unique.

Usage:
  passenger [command] [*args]

Commands:
  login     -l [passphrase]      : generate a JWT token to use other commands
  register  -r [passphrase]      : register a passphrase to the passenger client
  reset     -R [jwt] [new]       : reset the passphrase of the passenger client
  get       -g [jwt]             : list all entries without their passphrases
  query     -q [jwt] [keyword]   : search for a keyword in all entries
  save      -s [jwt] [data]      : store an entry with the given data
  fetch     -f [jwt] [id]        : retrieve an entry by its id
  update    -u [jwt] [id] [json] : update an entry by its id
  delete    -d [jwt] [id]        : delete an entry by its index
  help      -h --help            : show this help message and exit
  man       -m                   : show the manual page, if available
";
    public void Parse()
    {
      if (string.IsNullOrEmpty(command)) Error.MissingCommand();

      switch (command)
      {
        case "help" or "--help" or "-h": Help(); break;
        case "login" or "-l": Login(); break;
        case "register" or "-r": Register(); break;
        default: Error.UnknownCommand(); break;
      }
    }
    // Commands
    private void Help() => Console.Write(helpText);
    private void Login()
    {
      if (arguments.Length != 1)
        Error.ArgumentCount("login", 1);
      Console.WriteLine(authorization.GenerateToken(arguments[0]));
      Environment.Exit(0);
    }
    private void Register()
    {
      if (arguments.Length < 2) Error.ArgumentCount("register", 1);
    }
  }

  public static class Error
  {
    public static void ArgumentCount(string command, int minOrActual, int max = -1)
    {
      if (max == -1)
      {
        Console.WriteLine($"passenger: {command}: expected exactly {minOrActual} arguments");
      }
      else
      {
        Console.WriteLine($"passenger: {command}: expected {minOrActual}-{max} arguments");
      }
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