using System.Text.Json;

namespace Passenger
{
  public class Database
  {
    private readonly string databaseFile;
    private readonly DatabaseModel database;

    public Database(string username)
    {
      try
      {
        if (!Directory.Exists(OperatingSystem.StoragePath))
          Directory.CreateDirectory(OperatingSystem.StoragePath);

        databaseFile = Path.Combine(
          OperatingSystem.StoragePath,
          $"{username}.bus"
        );
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

    public void SaveToFile()
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

    public Credentials GetCredentials() => new()
    {
      Username = database.Username,
      Passphrase = database.Passphrase
    };

    public bool IsRegistered() =>
      !string.IsNullOrEmpty(database.Passphrase) &&
      !string.IsNullOrEmpty(database.Username);

    public void Register(string username, string passphrase)
    {
      database.Passphrase = passphrase;
      database.Username = username;
      database.Entries = [];
      SaveToFile();
    }

    public void ResetPassphrase(string oldPassphrase, string newPassphrase)
    {
      if (database.Passphrase != oldPassphrase) Error.AuthorizationFailed();
      database.Passphrase = newPassphrase;
      SaveToFile();
    }

    /*
     * CRUD operations
     */

    public ListableDatabaseEntry Create(ReadWritableDatabaseEntry entry)
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
    public string Import(List<DatabaseEntry> entries)
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

    public List<ListableDatabaseEntry> FetchAll() =>
      database.Entries.Select(
        Mapper.ToListable
      ).ToList();

    public DatabaseEntry Fetch(string id) =>
      database.Entries.Find(entry => entry.Id == id);

    public List<ListableDatabaseEntry> Query(string keyword) =>
      database.Entries.Where(entry =>
        (entry.Platform != null && entry.Platform.Contains(keyword)) ||
        (entry.Identity != null && entry.Identity.Contains(keyword)) ||
        (entry.Url != null && entry.Url.Contains(keyword))
      ).Select(Mapper.ToListable
      ).ToList();

    public ListableDatabaseEntry Update(string id, ReadWritableDatabaseEntry updatedEntry, bool preserveUpdatedAt = true)
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

    public void Delete(string id)
    {
      database.Entries.RemoveAll(entry => entry.Id == id);
      SaveToFile();
    }

    /*
     * Getters
     */

    public List<DatabaseEntry> AllEntries => database.Entries;

    public List<ReadWritableDatabaseEntry> AllReadWritableEntries =>
      database.Entries.Select(
        Mapper.ToReadWritable
      ).ToList();
  }
}
