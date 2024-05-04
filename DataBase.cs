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
        database = JsonSerializer.Deserialize<DatabaseModel>(data) ?? new DatabaseModel { Entries = new List<DatabaseEntry>() };
      }
      catch
      {
        Console.WriteLine("passenger: failed to load database");
        Environment.Exit(1);
      }
    }
    private static readonly string databaseFile = "./passenger.bus";
    private static readonly DatabaseModel database;
    public static void Save()
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
    public static void Append(DatabaseEntry entry)
    {
      entry = Validate.Entry(entry);
      // Auto-generate id
      entry.Id = Guid.NewGuid().ToString();
      entry.Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
      entry.Updated = entry.Created;
      // Append entry to database and save
      database.Entries.Add(entry);
      Save();
    }
    public static void Update(string id, DatabaseEntry entry)
    {
      var index = database.Entries.FindIndex(entry => entry.Id == id);
      if (index == -1)
      {
        Console.WriteLine("passenger: entry not found");
        Environment.Exit(1);
      }
      entry = Validate.Entry(entry);
      entry.Created = database.Entries[index].Created; // Preserve created date
      // Update entry in database and save
      database.Entries[index] = entry;
      Save();
    }
    public static void Delete(string id)
    {
      database.Entries.RemoveAll(entry => entry.Id == id);
      Save();
    }
    public static string GetAll() => JsonSerializer.Serialize(
      database.Entries.Select(entry => new SerializableDatabaseEntry
      {
        Id = entry.Id,
        Platform = entry.Platform,
        Username = entry.Username,
        Email = entry.Email
      })
    );
    public static string Query(string keyword) => JsonSerializer.Serialize(
      database.Entries.Where(entry =>
        (entry.Platform != null && entry.Platform.Contains(keyword)) ||
        (entry.Username != null && entry.Username.Contains(keyword)) ||
        (entry.Email != null && entry.Email.Contains(keyword))
      ).Select(entry => new SerializableDatabaseEntry
      { // Serialize only id, platform, username, and email
        Id = entry.Id,
        Platform = entry.Platform,
        Username = entry.Username,
        Email = entry.Email
      }).ToList()
    );
    public static string Fetch(string id) => JsonSerializer.Serialize(
      database.Entries.Find(entry => entry.Id == id)
    );

    public static string GetPassphrase() => database.Passphrase;
    public static bool IsRegistered()
    {
      if (File.Exists(databaseFile))
        return database.Passphrase != null;
      else return false;
    }
    public static void Register(string passphrase)
    {
      database.Passphrase = passphrase;
      database.Entries = [];
      Save();
    }
    public static void ResetPassphrase(string newPassphrase)
    {
      database.Passphrase = newPassphrase;
      Save();
    }
  }

  public class Validate
  {
    public static DatabaseEntry Entry(DatabaseEntry entry)
    {
      // Check if at at least email or username is provided
      if (string.IsNullOrEmpty(entry.Email) && string.IsNullOrEmpty(entry.Username))
      {
        Console.WriteLine("passenger: email or username is required");
        Environment.Exit(1);
      }
      // Check if required fields are provided
      if (string.IsNullOrEmpty(entry.Platform) || string.IsNullOrEmpty(entry.Passphrase))
      {
        Console.WriteLine("passenger: platform and passphrase are required");
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
    [JsonPropertyName("platform"), Required] // Required
    public string Platform { get; set; }
    [JsonPropertyName("username")] // Required if email is not provided
    public string Username { get; set; }
    [JsonPropertyName("email")] // Required if username is not provided
    public string Email { get; set; }
    [JsonPropertyName("passphrase"), Required] // Required
    public string Passphrase { get; set; }
    [JsonPropertyName("notes")] // Optional
    public string Notes { get; set; }
    [JsonPropertyName("created"), Required] // Auto-generated
    public string Created { get; set; }
    [JsonPropertyName("updated"), Required] // Auto-generated
    public string Updated { get; set; }
  }

  public class SerializableDatabaseEntry
  {
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("platform")]
    public string Platform { get; set; }
    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
  }
}
