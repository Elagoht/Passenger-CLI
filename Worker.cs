using System.Text.Json;

namespace Passenger
{
  /// <summary>
  /// Main work handler
  /// </summary>
  /// <param name="args"></param>
  /// <remarks>
  /// This class is responsible for handling the main work of the program.
  /// </remarks>
  public class Worker(string[] args)
  {
    private readonly Authorization authorization = new(EnDeCoder.JSWSecret);
    private readonly string[] arguments = args.Skip(1).ToArray();

    /*
     * Authorization
     */

    /// <summary>
    /// Login
    /// </summary>
    /// <remarks>
    /// Generate a JWT token to use other commands. Requires a passphrase.
    /// </remarks>
    public void Login()
    {
      if (arguments.Length != 2) Error.ArgumentCount("login", 2);
      Console.WriteLine(authorization.GenerateToken(arguments[0], arguments[1]));
      Environment.Exit(0);
    }

    /// <summary>
    /// Register
    /// </summary>
    /// <remarks>
    /// Register a passphrase to database.
    /// </remarks>
    public void Register()
    {
      if (arguments.Length != 2) Error.ArgumentCount("register", 2);
      if (Database.IsRegistered())
        Console.WriteLine("passenger: already registered");
      else
        Database.Register(arguments[0], arguments[1]);
    }

    /// <summary>
    /// Reset passphrase for accessing database
    /// </summary>
    /// <remarks>
    /// Reset the passphrase of the Passenger client using a JWT token and a new passphrase.
    /// </remarks>
    public void Reset()
    {
      if (arguments.Length != 2) Error.ArgumentCount("reset", 2);
      if (authorization.ValidateToken(arguments[0]))
        Database.ResetPassphrase(arguments[1]);
      else
        Error.InvalidToken();
    }

    /*
     * CRUD operations
     */

    /// <summary>
    /// Create a new entry
    /// </summary>
    /// <param name="entry"></param>
    /// <remarks>
    /// Store an entry with the given data, requires a JWT token.
    /// </remarks>
    public void Create()
    {
      if (arguments.Length != 2) Error.ArgumentCount("save", 2);
      if (authorization.ValidateToken(arguments[0]))
      {
        DatabaseEntry entry = JsonSerializer.Deserialize<DatabaseEntry>(arguments[1]);
        Validate.Entry(entry);
        Database.Create(entry);
      }
      else Error.InvalidToken();
    }

    /// <summary>
    /// Fetch all entries without passphrases
    /// </summary>
    /// <returns>A list of all entries</returns>
    /// <remarks>
    /// List all entries without displaying their passphrases, requires a JWT token.
    /// </remarks>
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
        Error.InvalidToken();
    }

    /// <summary>
    /// Fetch one entry by UUID
    /// </summary>
    /// <returns>DatabaseEntry</returns>
    /// <remarks>
    /// Retrieve an entry by its UUID, requires a JWT token.
    /// </remarks>
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
        Error.InvalidToken();
    }

    /// <summary>
    /// Query entries by keyword
    /// </summary>
    /// <returns>A list of entries</returns>
    /// <remarks>
    /// Search for a keyword in all entries, requires a JWT token.
    /// </remarks>
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
        Error.InvalidToken();
    }

    /// <summary>
    /// Update an entry
    /// </summary>
    /// <remarks>
    /// Update an entry by its UUID, requires a JWT token and JSON formatted data.
    /// </remarks>
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
        Error.InvalidToken();
    }

    /// <summary>
    /// Delete an entry
    /// </summary>
    /// <remarks>
    /// Delete an entry by its UUID, requires a JWT token.
    /// </remarks>
    public void Delete()
    {
      if (arguments.Length != 2) Error.ArgumentCount("delete", 2);
      if (authorization.ValidateToken(arguments[0]))
        Database.Delete(arguments[1]);
      else
        Error.InvalidToken();
    }

    /*
     * Help and manual
     */

    /// <summary>
    /// Show manual
    /// </summary>
    /// <remarks>
    /// Manual page with UNIX style, plain text to support Windows.
    /// </remarks>
    public static void Manual()
    {
      Console.WriteLine($@"PASSENGER(1)                 Passenger CLI Manual                 PASSENGER(1)

NAME
      Passenger - Portable and customizable password manager.

SYNOPSIS
      passenger [command] [*args]

DESCRIPTION
      Passenger is a command line password manager designed to be portable
      and customizable. It allows users to securely store, retrieve, manage,
      and generate passphrases using their own encode/decode algorithm,
      created from the open-source EnDeCoder.cs file. Each build of the
      Passenger client is unique, crafted by the user, ensuring a
      personalized security algorithm.

COMMANDS
      login -l
            Generate a JWT token to use other commands. Requires a
            passphrase.
            passenger login -l [passphrase]

      register -r
            Register a passphrase to the Passenger client.
            passenger register [passphrase]

      reset -R
            Reset the passphrase of the Passenger client using a JWT token
            and a new passphrase.
            passenger reset [jwt] [new]

      fetchAll -a
            List all entries without displaying their passphrases, requires
            a JWT token.
            passenger fetchAll [jwt]

      query -q
            Search for a keyword in all entries, requires a JWT token.
            passenger query [jwt] [keyword]

      fetch -f
            Retrieve an entry by its UUID, requires a JWT token.
            passenger fetch [jwt] [uuid]

      create -c
            Store an entry with the given data, requires a JWT token.
            passenger create [jwt] [json]

      update -u
            Update an entry by its UUID, requires a JWT token and JSON
            formatted json.
            passenger update [jwt] [uuid] [json]

      delete -d
            Delete an entry by its UUID, requires a JWT token.
            passenger delete [jwt] [uuid]

      version -v --version
            Show the version of the Passenger software.
            passenger version

      help -h --help
            Show this help message and exit.
            passenger help

      man -m Show the manual page, if available.
            passenger man

AUTHOR
      Written by Elagoht.

SEE ALSO
      jq(1)

{GlobalConstants.VERSION}                              May 2024                       PASSENGER(1)"
      );
      Environment.Exit(0);
    }

    /// <summary>
    /// Show help
    /// </summary>
    /// <remarks>
    /// Show help message and exit.
    /// </remarks>
    public static void Help()
    {
      Console.Write(@$"Passenger CLI {GlobalConstants.VERSION}
  Copyright (C) 2024 Elagoht
  
  Store, retrieve, manage and generate passphrases securely using your own encode/decode algorithm. Every passenger client is created by user's itself and unique.

Usage:
  passenger [command] [*args]

Commands:
  login     -l [username] [passphrase] : generate a JWT token to use other commands
  register  -r [username] [passphrase] : register a passphrase to the passenger client
  reset     -R [jwt] [new]             : reset the passphrase of the passenger client
  fetchAll  -a [jwt]                   : list all entries without their passphrases
  query     -q [jwt] [keyword]         : list search results without their passphrases
  fetch     -f [jwt] [uuid]            : retrieve an entry by its uuid with its passphrase
  create    -c [jwt] [json]            : store an entry with the given json
  update    -u [jwt] [uuid] [json]     : update an entry by its uuid
  delete    -d [jwt] [uuid]            : delete an entry by its index
  version   -v --version               : show the version and exit
  help      -h --help                  : show this help message and exit
  man       -m                         : show the manual page, if available
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