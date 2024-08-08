using System.Text.Json;

namespace Passenger
{
  public class Worker(string[] args, string piped)
  {
    private readonly Authorization authorization = new(EnDeCoder.JSWSecret);
    private readonly string[] arguments = args.Skip(1).ToArray();
    private readonly string piped = piped;
    private Database Database;

    private void RoutineAuthControl(string verbName, int minOrActual, int max = -1)
    {
      string token = Environment.GetEnvironmentVariable("JWT");

      if (arguments.Length < minOrActual || (max != -1 && arguments.Length > max))
        Error.ArgumentCount(verbName, minOrActual, max);
      if (!authorization.ValidateToken(token))
        Error.AuthorizationFailed();

      // Initialize database
      Database = new Database(Authorization.GetUserName(token));
    }

    private void RequirePipedInput()
    {
      if (piped == null) Error.PipedInputRequired();
    }

    /*
     * Authorization
     */

    public void Login()
    {
      if (arguments.Length != 2) Error.ArgumentCount("login", 2);
      Console.WriteLine(authorization.GenerateToken(arguments[0], arguments[1]));
      Environment.Exit(0);
    }

    public void Register()
    {
      if (arguments.Length != 2) Error.ArgumentCount("register", 2);
      // Routine control is not called here, so initialize database manually
      Database = new Database(arguments[0]);
      if (Database.IsRegistered())
        Console.WriteLine("passenger: already registered");
      else
        Database.Register(arguments[0], arguments[1]);
    }

    public void Reset()
    {
      RoutineAuthControl("reset", 2);
      Database.ResetPassphrase(arguments[0], arguments[1]);
    }

    /*
     * CRUD operations
     */

    public void Create()
    {
      RoutineAuthControl("create", 1);
      Console.WriteLine(JsonSerializer.Serialize(
        Database.Create(Validate.JsonAsDatabaseEntry(arguments[0]))
      ));
    }

    public void FetchAll()
    {
      RoutineAuthControl("fetchAll", 0);
      Console.WriteLine(JsonSerializer.Serialize(
        Database.FetchAll()
      ));
    }

    public void Fetch()
    {
      RoutineAuthControl("fetch", 1);
      /**
       * Fetch can read passphrase history as well.
       * Don't worry, it's not a security issue.
       * Because passphrase history only tracks
       * the stats of the passphrase, not the
       * passphrase itself.
       */
      DatabaseEntry entry = Database.Fetch(arguments[0]);
      if (entry == null) Error.EntryNotFound();
      entry.TotalAccesses++;
      Database.Update(entry.Id, entry, false);
      Console.WriteLine(JsonSerializer.Serialize(entry));
    }

    public void Query()
    {
      RoutineAuthControl("query", 1);
      Console.WriteLine(
        JsonSerializer.Serialize(
          Database.Query(arguments[0])
        )
      );
    }

    public void Update()
    {
      RoutineAuthControl("update", 2);
      ReadWritableDatabaseEntry entry = Validate.JsonAsDatabaseEntry(arguments[1]);
      Validate.Entry(entry);
      Console.WriteLine(JsonSerializer.Serialize(
        Database.Update(arguments[0], entry)
      ));
    }

    public void Delete()
    {
      RoutineAuthControl("delete", 1);
      Database.Delete(arguments[0]);
    }

    /*
     * Statistics
     */

    public void Statistics()
    {
      RoutineAuthControl("stats", 0);
      Statistics statistics = new(Database.AllReadWritableEntries);
      DashboardData dashboardData = new()
      {
        TotalCount = statistics.TotalCount,
        UniquePlatforms = statistics.UniquePlatforms,
        UniquePlatformsCount = statistics.UniquePlatformsCount,
        UniquePassphrases = statistics.UniquePassphrases,
        MostAccessed = statistics.MostAccessed,
        CommonByPlatform = statistics.CommonByPlatform,
        AverageLength = statistics.AverageLength,
        PercentageOfCommon = statistics.PercentageOfCommon,
        MostCommon = statistics.MostCommon,
        Strengths = statistics.Strengths,
        AverageStrength = statistics.AverageStrength,
        WeakPassphrases = statistics.WeakPassphrases,
        MediumPassphrases = statistics.MediumPassphrases,
        StrongPassphrases = statistics.StrongPassphrases
      };
      Console.WriteLine(JsonSerializer.Serialize(dashboardData));
    }

    public void Detect()
    {
      RoutineAuthControl("detect", 0);
      Detective detective = new(Database.AllEntries);
      Console.WriteLine(JsonSerializer.Serialize(detective));
    }

    /*
     * Data transfer
     */

    public void Import()
    {
      RoutineAuthControl("import", 1);
      RequirePipedInput();
      // Check if browser typeis supported
      if (!Browser.SupportedBrowsers.Contains(arguments[0]))
        Error.BrowserTypeNotSupported();

      // Get imported and skipped entries
      List<DatabaseEntry>[] data = Browser.Import(arguments[0], piped);
      List<DatabaseEntry> mappedEntries = data[0];
      List<DatabaseEntry> skippedEntries = data[1];

      if (skippedEntries.Count > 0)
        Error.ImportHasBadEntries(skippedEntries, mappedEntries);

      Console.WriteLine(Database.Import(mappedEntries));
    }

    public void Export()
    {
      RoutineAuthControl("export", 1);
      if (!Browser.exportTypes.Contains(arguments[0]))
        Error.ExportTypeNotSupported();
      Console.WriteLine(Browser.Export(
        arguments[0], Database.AllReadWritableEntries
      ));
    }

    /*
     * Generation
     */

    public void Generate()
    {
      switch (arguments.Length)
      {
        case 0:
          Console.WriteLine(Generator.New()); break;
        case 1:
          Console.WriteLine(Generator.New(int.Parse(arguments[0]))); break;
        default:
          Error.ArgumentCount("generate", 0, 1);
          break;
      }
    }

    public void Manipulate()
    {
      if (arguments.Length != 1) Error.ArgumentCount("manipulate", 1);
      Console.WriteLine(Generator.Manipulated(arguments[0]));
    }

    /*
     * Help and manual
     */

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
            Generate a JWT to use other commands. Requires a
            passphrase.
            passenger login -l [username] [passphrase]

      register -r
            Register a passphrase to the Passenger client.
            passenger register [username] [passphrase]

      reset -R
            Reset the passphrase of the Passenger client using a JWT
            and a new passphrase.
            JWT=[jwt] passenger reset [old] [new]

      fetchAll -a
            List all entries without displaying their passphrases, requires
            a JWT.
            JWT=[jwt] passenger fetchAll

      query -q
            Search for a keyword in all entries, requires a JWT.
            JWT=[jwt] passenger query [keyword]

      fetch -f
            Retrieve an entry by its UUID, requires a JWT.
            JWT=[jwt] passenger fetch [uuid]

      create -c
            Store an entry with the given json, requires a JWT.
            JWT=[jwt] passenger create [json]

      update -u
            Update an entry by its UUID, requires a JWT and JSON
            formatted json.
            JWT=[jwt] passenger update [uuid] [json]

      delete -d
            Delete an entry by its UUID, requires a JWT.
            JWT=[jwt] passenger delete [uuid]

      stats -s
            Show statistics of the database.
            JWT=[jwt] passenger stats

      detect -d
            Detect issues about the security of passphrases, requires a JWT
            token.
            JWT=[jwt] passenger detect

      import -i
            Import a CSV file from a browser, requires a JWT.
            Browser can be chromium, firefox, or safari.
            You can export your passwords as a CSV file from your browser.
            Accepts the CSV content as piped input to support custom clients.

            If your csv file have entries that Passenger would not accept,
            you will get the skipped entries on stderr, and acceptable entries
            on stdout. Acceptable entries will be stored in the database unless
            you add them again.

            Accaptable entries can be piped to a file to import from it.

            cat passwords.csv | JWT=[jwt] passenger import [browser]

      export -e
            Export the database to a CSV file, requires a JWT.
            Method can be bare or encrypted. Base64 encryption will be used.
            Writes to stdout, can be redirected to a file.
            JWT=[jwt] passenger export [method]

      generate -g
            Generate a passphrase with the given length.
            Default length is 32.
            passenger generate [length]

      manipulate -m
            Manipulate a passphrase by changing its characters.
            Still recognizable by humans.
            passenger manipulate [passphrase]

      version -v --version
            Show the version of the Passenger software.
            passenger version

      help -h --help
            Show this help message and exit.
            passenger help

      man -M
            Show the manual page, if available.
            passenger man

AUTHOR
      Written by Elagoht.

SEE ALSO
      jq(1)

{GlobalConstants.VERSION}                              Aug 2024                       PASSENGER(1)"
      );
      Environment.Exit(0);
    }

    public static void Help()
    {
      Console.Write(@$"Passenger CLI {GlobalConstants.VERSION}
  Copyright (C) 2024 Elagoht

  Store, retrieve, manage and generate passphrases securely using your own encode/decode algorithm. Every passenger client is created by user's itself and unique.

Usage:
  passenger [command] [*args]
  SECRET_KEY=[key] passenger [command] [*args]
  SECRET_KEY=[key] JWT=[jwt] passenger [command] [*args]

Commands:
  login      -l [username] [passphrase] : generate a JWT to use other commands
  register   -r [username] [passphrase] : register a passphrase to the passenger client
  reset      -R [old] [new]             : reset the passphrase of the passenger client
  fetchAll   -a                         : list all entries without their passphrases
  query      -q [keyword]               : list search results without their passphrases
  fetch      -f [uuid]                  : retrieve an entry by its uuid with its passphrase
  create     -c [json]                  : store an entry with the given json
  update     -u [uuid] [json]           : update an entry by its uuid
  delete     -d [uuid]                  : delete an entry by its index
  stats      -s                         : show statistics of the database
  detect     -d                         : detect issues about security of passphrases
  import     -i [browser]               : import `chromium`, `firefox` or `safari` csv
  export     -e [method]                : export to `bare` or `encrypted` csv
  generate   -g [length]                : generate a passphrase with the given length
  manipulate -m [passphrase]            : manipulate a passphrase
  version    -v --version               : show the version and exit
  help       -h --help                  : show this help message and exit
  man        -M                         : show the manual page, if available
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