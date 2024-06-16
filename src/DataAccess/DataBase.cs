using System.Text.Json;

namespace Passenger
{
  public static class Database
  {
    static Database()
    {
      try
      {
        string data = File.Exists(databaseFile)
          ? FileSystem.Read(databaseFile)
          : "{}";
        database = JsonSerializer.Deserialize<DatabaseModel>(data)
          ?? new DatabaseModel { Entries = [] };
      }
      catch { Error.DatabaseLoadFailed(); }
    }

    /*
     * Database file
     */

    private static readonly string databaseFile = "./passenger.bus";
    private static readonly DatabaseModel database;

    public static void SaveToFile()
    {
      try
      {
        string jsonData = JsonSerializer.Serialize(database);
        FileSystem.Write(databaseFile, jsonData);
      }
      catch
      {
        Console.WriteLine("passenger: failed to save database");
        Environment.Exit(1);
      }
    }

    /*
     * Authorization methods
     */

    public static Credentials GetCredentials() => new()
    {
      Username = database.Username,
      Passphrase = database.Passphrase
    };

    public static bool IsRegistered() =>
      !string.IsNullOrEmpty(database.Passphrase) &&
      !string.IsNullOrEmpty(database.Username);

    public static void Register(string username, string passphrase)
    {
      database.Passphrase = passphrase;
      database.Username = username;
      database.Entries = [];
      database.Constants = [];
      SaveToFile();
    }

    public static void ResetPassphrase(string newPassphrase)
    {
      database.Passphrase = newPassphrase;
      SaveToFile();
    }

    /*
     * CRUD operations
     */

    public static string Create(ReadWritableDatabaseEntry entry)
    {
      Validate.Entry(entry);
      // Auto-generate id
      entry.Id = Guid.NewGuid().ToString();
      entry.Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      entry.Updated = entry.Created;
      // Append entry to database and save
      database.Entries.Add(Mapper.NewlyCreated(entry));
      SaveToFile();
      return JsonSerializer.Serialize(entry);
    }

    public static List<ListableDatabaseEntry> FetchAll() =>
      database.Entries.Select(
        Mapper.ToListable
      ).ToList();

    public static ReadWritableDatabaseEntry FetchOne(string id) =>
      Mapper.ToReadWritable(
        database.Entries.Find(entry => entry.Id == id)
      );

    public static List<ListableDatabaseEntry> Query(string keyword) =>
      database.Entries.Where(entry =>
        (entry.Platform != null && entry.Platform.Contains(keyword)) ||
        (entry.Identity != null && entry.Identity.Contains(keyword)) ||
        (entry.Url != null && entry.Url.Contains(keyword))
      ).Select(Mapper.ToListable
      ).ToList();

    public static ListableDatabaseEntry Update(string id, ReadWritableDatabaseEntry updatedEntry, bool preserveUpdatedAt = true)
    {
      int index = database.Entries.FindIndex(entry => entry.Id == id);
      if (index == -1) Error.EntryNotFound();
      DatabaseEntry existingEntry = database.Entries[index];

      // Update changes
      existingEntry.Platform = updatedEntry.Platform;
      existingEntry.Url = updatedEntry.Url;
      existingEntry.Identity = updatedEntry.Identity;
      existingEntry.Notes = updatedEntry.Notes;
      existingEntry.TotalAccesses = updatedEntry.TotalAccesses;
      // Preserve the updated at timestamp if requested
      existingEntry.Updated = preserveUpdatedAt
        ? existingEntry.Updated
        : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      // If passphrase is updated, append to history
      if (existingEntry.Passphrases.Last().Passphrase != updatedEntry.Passphrase)
        existingEntry.Passphrases = Mapper.RegisterNewPassphrase(
          existingEntry.Passphrases,
          updatedEntry.Passphrase
        );
      SaveToFile();

      return Mapper.ToListable(existingEntry);
    }

    public static void Delete(string id)
    {
      database.Entries.RemoveAll(entry => entry.Id == id);
      SaveToFile();
    }

    /*
     * Constants pair methods
     */

    public static ConstantPair DeclareConstant(ConstantPair entry)
    {
      Validate.ConstantPair(entry);
      database.Constants ??= [];
      database.Constants.Add(entry);
      SaveToFile();
      return entry;
    }

    public static ConstantPair FetchConstant(string constant) =>
      (database.Constants ?? []).Find(pair =>
        pair.Key == constant
      );

    public static ConstantPair ModifyConstant(string key, ConstantPair newPair)
    {
      if (FetchConstant(key) == null) Error.EntryNotFound();
      Validate.ConstantPair(newPair, false);
      database.Constants[database.Constants.FindIndex(pair =>
        pair.Key == key
      )] = newPair;
      SaveToFile();
      return newPair;
    }

    public static void ForgetConstant(string constant)
    {
      if (FetchConstant(constant) == null) Error.EntryNotFound();
      database.Constants.RemoveAll((pair) =>
        pair.Key == constant
      );
      SaveToFile();
    }

    /*
     * Getters
     */

    public static List<ConstantPair> AllConstants => database.Constants ?? [];

    public static List<DatabaseEntry> AllEntries => database.Entries;

    public static List<ReadWritableDatabaseEntry> AllReadWritableEntries =>
      database.Entries.Select(
        Mapper.ToReadWritable
      ).ToList();
  }
}
