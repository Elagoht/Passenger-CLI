
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
        database = JsonSerializer.Deserialize<DatabaseModel>(
          File.Exists(databaseFile)
          ? FileSystem.Read(databaseFile)
          : "{}"
        );
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
      entry = ValidateEntry(entry);
      // Auto-generate id
      entry.Id = Guid.NewGuid().ToString();
      entry.Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
      entry = ValidateEntry(entry);
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
    public static DatabaseEntry ValidateEntry(DatabaseEntry entry)
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
}
