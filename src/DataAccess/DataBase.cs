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

    public static ListableDatabaseEntry Create(ReadWritableDatabaseEntry entry)
    {
      Validate.Entry(entry);
      DatabaseEntry savedEntry = Mapper.CreateDatabaseEntry(
        entry.Platform, entry.Identity, entry.Url,
        entry.Passphrase, entry.Notes
      );
      database.Entries.Add(savedEntry);
      SaveToFile();
      return Mapper.ToListable(savedEntry);
    }

    /// <summary>Prevents re-reading and re-crypting the database file</summary>
    public static string Import(List<DatabaseEntry> entries)
    {
      try
      {
        entries.ForEach(database.Entries.Add);
        SaveToFile();
        return $"Imported {entries.Count} {(entries.Count == 1
          ? "entry"
          : "entries"
        )} successfully.";
      }
      catch { Error.ImportFailed(); throw; }
    }

    public static List<ListableDatabaseEntry> FetchAll() =>
      database.Entries.Select(
        Mapper.ToListable
      ).ToList();

    public static DatabaseEntry Fetch(string id) =>
        database.Entries.Find(entry => entry.Id == id);

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

      // Update fields
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
      if (existingEntry.Passphrase != updatedEntry.Passphrase)
        existingEntry.PassphraseHistory.Add(
          new()
          {
            Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Length = updatedEntry.Passphrase.Length,
            Strength = Strength.Calculate(updatedEntry.Passphrase)
          }
        );
      // After updating history, finally update the passphrase
      existingEntry.Passphrase = updatedEntry.Passphrase;
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
