using System.Text.Json;

namespace Passenger
{
  public class Worker(string[] args)
  {
    private readonly Authorization authorization = new(EnDeCoder.JSWSecret);
    private readonly string[] arguments = args.Skip(1).ToArray();

    public void Login()
    {
      if (arguments.Length != 1) Error.ArgumentCount("login", 1);
      Console.WriteLine(authorization.GenerateToken(arguments[0]));
      Environment.Exit(0);
    }
    public void Register()
    {
      if (arguments.Length != 1) Error.ArgumentCount("register", 1);
      if (Database.IsRegistered())
        Console.WriteLine("passenger: already registered");
      else
        Database.Register(arguments[0]);
    }
    public void Reset()
    {
      if (arguments.Length != 2) Error.ArgumentCount("reset", 2);
      if (authorization.ValidateToken(arguments[0]))
        Database.ResetPassphrase(arguments[1]);
      else
        Console.WriteLine("passenger: invalid token");
    }
    public void Get()
    {
      if (arguments.Length != 1) Error.ArgumentCount("get", 1);
      if (authorization.ValidateToken(arguments[0]))
        Console.WriteLine(Database.GetAll());
      else
        Console.WriteLine("passenger: invalid token");
    }
    public void Query()
    {
      if (arguments.Length != 2) Error.ArgumentCount("query", 2);
      if (authorization.ValidateToken(arguments[0]))
        Console.WriteLine(Database.Query(arguments[1]));
      else
        Console.WriteLine("passenger: invalid token");
    }
    public void Save()
    {
      if (arguments.Length != 2) Error.ArgumentCount("save", 2);
      if (authorization.ValidateToken(arguments[0]))
      {
        DatabaseEntry entry = JsonSerializer.Deserialize<DatabaseEntry>(arguments[1]);
        Database.ValidateEntry(entry);
        Database.Append(entry);
      }
      else Console.WriteLine("passenger: invalid token");
    }
    public static void Help()
    {
      Console.Write(@"Passenger CLI 0.1.0
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
");
      Environment.Exit(0);
    }
  }
}