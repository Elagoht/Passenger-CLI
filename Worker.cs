using System.Diagnostics;
using System.Text.Json;

namespace Passenger
{
  public class Worker(string[] args)
  {
    private readonly Authorization authorization = new(EnDeCoder.JSWSecret);
    private readonly string[] arguments = args.Skip(1).ToArray();

    // *** Authorization *** //
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

    // *** CRUD operations *** //
    public void Create()
    {
      if (arguments.Length != 2) Error.ArgumentCount("save", 2);
      if (authorization.ValidateToken(arguments[0]))
      {
        DatabaseEntry entry = JsonSerializer.Deserialize<DatabaseEntry>(arguments[1]);
        Validate.Entry(entry);
        Database.Create(entry);
      }
      else Console.WriteLine("passenger: invalid token");
    }

    public void FetchAll()
    {
      if (arguments.Length != 1) Error.ArgumentCount("get", 1);
      if (authorization.ValidateToken(arguments[0]))
        Console.WriteLine(
          JsonSerializer.Serialize(
            Database.FetchAll()
          )
        );
      else
        Console.WriteLine("passenger: invalid token");
    }

    public void FetchOne()
    {
      if (arguments.Length != 2) Error.ArgumentCount("fetch", 2);
      if (authorization.ValidateToken(arguments[0]))
        Console.WriteLine(
          JsonSerializer.Serialize(
            Database.FetchOne(arguments[1])
          )
        );
      else
        Console.WriteLine("passenger: invalid token");
    }

    public void Query()
    {
      if (arguments.Length != 2) Error.ArgumentCount("query", 2);
      if (authorization.ValidateToken(arguments[0]))
        Console.WriteLine(
          JsonSerializer.Serialize(
            Database.Query(arguments[1])
          )
        );
      else
        Console.WriteLine("passenger: invalid token");
    }

    public void Update()
    {
      if (arguments.Length != 3) Error.ArgumentCount("update", 3);
      if (authorization.ValidateToken(arguments[0]))
      {
        DatabaseEntry entry = JsonSerializer.Deserialize<DatabaseEntry>(arguments[2]);
        Validate.Entry(entry);
        Database.Update(arguments[1], entry);
      }
      else
        Console.WriteLine("passenger: invalid token");
    }

    public void Delete()
    {
      if (arguments.Length != 2) Error.ArgumentCount("delete", 2);
      if (authorization.ValidateToken(arguments[0]))
        Database.Delete(arguments[1]);
      else
        Console.WriteLine("passenger: invalid token");
    }

    // *** Help and manual *** //
    public static void Manual()
    {
      // Open manual page in man command
      Process.Start("man", "./passenger-pm.1");
    }

    public static void Help()
    {
      Console.Write(@$"Passenger CLI {GlobalConstants.VERSION}
  Copyright (C) 2024 Elagoht
  
  Store, retrieve, manage and generate passphrases securely using your own encode/decode algorithm. Every passenger client is created by user's itself and unique.

Usage:
  passenger [command] [*args]

Commands:
  login     -l [passphrase]        : generate a JWT token to use other commands
  register  -r [passphrase]        : register a passphrase to the passenger client
  reset     -R [jwt] [new]         : reset the passphrase of the passenger client
  fetchAll  -a [jwt]               : list all entries without their passphrases
  query     -q [jwt] [keyword]     : list search results without their passphrases
  fetch     -f [jwt] [uuid]        : retrieve an entry by its uuid with its passphrase
  create    -c [jwt] [data]        : store an entry with the given data
  update    -u [jwt] [uuid] [json] : update an entry by its uuid
  delete    -d [jwt] [uuid]        : delete an entry by its index
  version   -v --version           : show the version and exit
  help      -h --help              : show this help message and exit
  man       -m                     : show the manual page, if available
");
      Environment.Exit(0);
    }

    // *** Version *** //
    public static void Version()
    {
      Console.WriteLine($"Passenger CLI {GlobalConstants.VERSION}");
      Environment.Exit(0);
    }
  }
}