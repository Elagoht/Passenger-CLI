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

    // *** Database file *** //
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

    // *** Authorization *** //
    public static string GetPassphrase() => database.Passphrase;

    public static bool IsRegistered() => !string.IsNullOrEmpty(database.Passphrase);

    public static void Register(string passphrase)
    {
      database.Passphrase = passphrase;
      database.Entries = [];
      SaveToFile();
    }

    public static void ResetPassphrase(string newPassphrase)
    {
      database.Passphrase = newPassphrase;
      SaveToFile();
    }

    // *** CRUD operations *** //
    public static void Create(DatabaseEntry entry)
    {
      entry = Validate.Entry(entry);
      // Auto-generate id
      entry.Id = Guid.NewGuid().ToString();
      entry.Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      entry.Updated = entry.Created;
      // Append entry to database and save
      database.Entries.Add(entry);
      SaveToFile();
    }

    public static List<ListableDatabaseEntry> FetchAll() => database.Entries.Select(entry =>
      new ListableDatabaseEntry
      {
        Id = entry.Id,
        Platform = entry.Platform,
        Idendity = entry.Identity,
        Url = entry.Url
      }
    ).ToList();

    public static DatabaseEntry FetchOne(string id) => database.Entries
      .Find(entry => entry.Id == id);

    public static List<ListableDatabaseEntry> Query(string keyword) => database.Entries.Where(entry =>
        (entry.Platform != null && entry.Platform.Contains(keyword)) ||
        (entry.Identity != null && entry.Identity.Contains(keyword)) ||
        (entry.Url != null && entry.Url.Contains(keyword))
      ).Select(entry => new ListableDatabaseEntry
      {
        Id = entry.Id,
        Platform = entry.Platform,
        Idendity = entry.Identity,
        Url = entry.Url
      }
    ).ToList();

    public static void Update(string id, DatabaseEntry entry)
    {
      var index = database.Entries.FindIndex(entry => entry.Id == id);
      if (index == -1) Error.EntryNotFound();
      entry = Validate.Entry(entry);

      entry.Id = database.Entries[index].Id; // Preserve id
      entry.Created = database.Entries[index].Created; // Preserve created date
      entry.Updated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      // Update entry in database and save
      database.Entries[index] = entry;
      SaveToFile();
    }

    public static void Delete(string id)
    {
      database.Entries.RemoveAll(entry => entry.Id == id);
      SaveToFile();
    }
  }

  public class Validate
  {
    public static DatabaseEntry Entry(DatabaseEntry entry)
    {
      // Check if required fields are provided
      if (string.IsNullOrEmpty(entry.Platform)) Error.MissingField("platform");
      if (string.IsNullOrEmpty(entry.Passphrase)) Error.MissingField("passphrase");
      if (string.IsNullOrEmpty(entry.Url)) Error.MissingField("url");
      if (string.IsNullOrEmpty(entry.Identity)) Error.MissingField("identity");
      {
        Environment.Exit(1);
      }
      // Auto-generate created and updated
      entry.Updated = entry.Created;
      return entry;
    }
  }

  public class DatabaseModel
  {
    [JsonPropertyName("passphrase")]
    public string Passphrase { get; set; }
    [JsonPropertyName("entries")]
    public List<DatabaseEntry> Entries { get; set; }
  }

  public class DatabaseEntry
  {
    [JsonPropertyName("id"), Required] // Auto-generated
    public string Id { get; set; }
    [JsonPropertyName("platform"), Required]
    public string Platform { get; set; }
    [JsonPropertyName("url"), Required]
    public string Url { get; set; }
    [JsonPropertyName("identity"), Required]
    public string Identity { get; set; }
    [JsonPropertyName("passphrase"), Required]
    public string Passphrase { get; set; }
    [JsonPropertyName("notes")] // Optional
    public string Notes { get; set; }
    [JsonPropertyName("created"), Required] // Auto-generated
    public string Created { get; set; }
    [JsonPropertyName("updated"), Required] // Auto-generated
    public string Updated { get; set; }
  }

  public class ListableDatabaseEntry
  {
    [JsonPropertyName("id"), Required]
    public string Id { get; set; }
    [JsonPropertyName("platform"), Required]
    public string Platform { get; set; }
    [JsonPropertyName("identity"), Required]
    public string Idendity { get; set; }
    [JsonPropertyName("url"), Required]
    public string Url { get; set; }
  }
}
