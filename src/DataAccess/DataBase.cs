using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Passenger
{
  public static class Database
  {
    static Database()
    {
      try
      {
        string data = File.Exists(databaseFile) ? FileSystem.Read(databaseFile) : "{}";
        database = JsonSerializer.Deserialize<DatabaseModel>(data) ?? new DatabaseModel { Entries = [] };
      }
      catch
      {
        Console.WriteLine("passenger: failed to load database");
        Environment.Exit(1);
      }
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

    public static string Create(DatabaseEntry entry)
    {
      entry = Validate.Entry(entry);
      // Auto-generate id
      entry.Id = Guid.NewGuid().ToString();
      entry.Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      entry.Updated = entry.Created;
      // Append entry to database and save
      database.Entries ??= [];
      database.Entries.Add(entry);
      SaveToFile();
      return JsonSerializer.Serialize(entry);
    }

    public static List<ListableDatabaseEntry> FetchAll() => database.Entries.Select(
      ConvertEntryToListable
    ).ToList();

    public static DatabaseEntry FetchOne(string id)
    {
      DatabaseEntry entry = database.Entries.Find(entry => entry.Id == id);
      entry.Identity = ResolveConstant(entry.Identity);
      return entry;
    }

    public static List<ListableDatabaseEntry> Query(string keyword) => database.Entries.Where(entry =>
        (entry.Platform != null && entry.Platform.Contains(keyword)) ||
        (entry.Identity != null && entry.Identity.Contains(keyword)) ||
        (entry.Url != null && entry.Url.Contains(keyword))
      ).Select(ConvertEntryToListable
      ).ToList();

    public static void Update(string id, DatabaseEntry entry, bool preserveUpdated = true)
    {
      int index = database.Entries.FindIndex(entry => entry.Id == id);
      if (index == -1) Error.EntryNotFound();
      entry = Validate.Entry(entry);

      // Protect private fields
      DatabaseEntry currentEntry = database.Entries[index];
      entry.Id = currentEntry.Id;
      entry.Created = currentEntry.Created;
      entry.Updated = preserveUpdated
        ? currentEntry.Updated
        : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      entry.TotalAccesses = currentEntry.TotalAccesses; // Do not increase
      database.Entries[index] = entry;
      SaveToFile();
    }

    public static void Delete(string id)
    {
      database.Entries.RemoveAll(entry => entry.Id == id);
      SaveToFile();
    }

    /*
     * Constants pair methods
     */

    public static void DeclareConstant(ConstantPair entry)
    {
      Validate.ConstantPair(entry);
      database.Constants ??= [];
      database.Constants.Add(entry);
      SaveToFile();
    }

    public static ConstantPair FetchConstant(string constant) =>
      (database.Constants ?? []).Find(pair =>
        pair.Key == constant
      );

    public static void ForgetConstant(string constant)
    {
      if (FetchConstant(constant) == null) Error.EntryNotFound();
      database.Constants.RemoveAll((pair) =>
        pair.Key == constant
      );
      SaveToFile();
    }

    public static List<ConstantPair> AllConstants => database.Constants ?? [];

    public static string ResolveConstant(string key) =>
      AllConstants.Find((pair) =>
        $"_${pair.Key}" == key
      )?.Value ?? key;

    /*
     * Conversion methods
     */

    public static DatabaseEntry[] AllEntries => [.. database.Entries];

    public static ListableDatabaseEntry ConvertEntryToListable(DatabaseEntry entry) => new()
    {
      Id = entry.Id,
      Platform = entry.Platform,
      Identity = ResolveConstant(entry.Identity),
      Url = entry.Url,
      Created = entry.Created,
      Updated = entry.Updated,
      TotalAccesses = entry.TotalAccesses
    };

    public static CountableDatabaseEntry ConvertEntryToCountable(DatabaseEntry entry) => new()
    {
      Platform = entry.Platform,
      Id = entry.Id,
      Url = entry.Url,
      TotalAccesses = entry.TotalAccesses
    };

    public static MicroDatabaseEntry ConvertEntryToMicro(DatabaseEntry entry) => new()
    {
      Platform = entry.Platform,
      Id = entry.Id,
      Url = entry.Url
    };
  }

  public class DatabaseModel
  {

    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("passphrase")]
    public string Passphrase { get; set; }
    [JsonPropertyName("entries")]
    public List<DatabaseEntry> Entries { get; set; }
    [JsonPropertyName("constants")]
    public List<ConstantPair> Constants { get; set; }
  }

  public class ConstantPair
  {
    [JsonPropertyName("key"), Required]
    public string Key { get; set; }
    [JsonPropertyName("value"), Required]
    public string Value { get; set; }
  }

  public class MicroDatabaseEntry
  {
    [JsonPropertyName("platform"), Required]
    public string Platform { get; set; }
    [JsonPropertyName("id"), Required]
    public string Id { get; set; }
    [JsonPropertyName("url"), Required]
    public string Url { get; set; }
  }

  public class CountableDatabaseEntry : MicroDatabaseEntry
  {
    [JsonPropertyName("totalAccesses"), Required]
    public int TotalAccesses { get; set; }
  }

  public class ListableDatabaseEntry : CountableDatabaseEntry
  {
    [JsonPropertyName("identity"), Required]
    public string Identity { get; set; }
    [JsonPropertyName("created"), Required] // Auto-generated
    public string Created { get; set; }
    [JsonPropertyName("updated"), Required] // Auto-generated
    public string Updated { get; set; }
  }

  public class DatabaseEntry : ListableDatabaseEntry
  {
    [JsonPropertyName("passphrase"), Required]
    public string Passphrase { get; set; }
    [JsonPropertyName("notes")] // Optional
    public string Notes { get; set; }
  }

  public class Credentials
  {
    public string Username { get; set; }
    public string Passphrase { get; set; }
  }
}
