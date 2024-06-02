using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Passenger
{
  /// <summary>
  /// Database operations for Passenger
  /// </summary>
  /// <remarks>
  /// This class provides CRUD operations for the database.
  /// </remarks>
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

    /// <summary>
    /// Save database to file
    /// </summary>
    /// <remarks>
    /// This method serializes the database object to JSON and writes it to the database file.
    /// </remarks>
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

    /// <summary>
    /// Get passphrase
    /// </summary>
    /// <returns>Passphrase</returns>
    /// <remarks>
    /// This method returns the passphrase stored in the database.
    /// </remarks>
    public static Credentials GetCredentials() => new()
    {
      Username = database.Username,
      Passphrase = database.Passphrase
    };

    /// <summary>
    /// Check if registered
    /// </summary>
    /// <returns>True if registered, false otherwise</returns>
    /// <remarks>
    /// This method checks if the database has a passphrase.
    /// </remarks>
    public static bool IsRegistered() =>
      !string.IsNullOrEmpty(database.Passphrase) &&
      !string.IsNullOrEmpty(database.Username);

    /// <summary>
    /// Register
    /// </summary>
    /// <param name="passphrase">Passphrase</param>
    /// <remarks>
    /// This method registers a passphrase in the database.
    /// </remarks>
    public static void Register(string username, string passphrase)
    {
      database.Passphrase = passphrase;
      database.Username = username;
      database.Entries = [];
      database.Constants = [];
      SaveToFile();
    }

    /// <summary>
    /// Reset passphrase
    /// </summary>
    /// <param name="newPassphrase">New passphrase</param>
    /// <remarks>
    /// This method resets the passphrase of the database.
    /// </remarks>
    /// 
    public static void ResetPassphrase(string newPassphrase)
    {
      database.Passphrase = newPassphrase;
      SaveToFile();
    }

    /*
     * CRUD operations
     */

    /// <summary>
    /// Create entry
    /// </summary>
    /// <param name="entry">Database entry</param>
    /// <remarks>
    /// This method creates a new entry in the database.
    /// </remarks>
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

    /// <summary>
    /// Fetch all entries
    /// </summary>
    /// <returns>List of listable database entries</returns>
    /// <remarks>
    /// This method fetches all entries from the database.
    /// </remarks>
    public static List<ListableDatabaseEntry> FetchAll() => database.Entries.Select(
      ConvertEntryToListable
    ).ToList();

    /// <summary>
    /// Fetch one entry
    /// </summary>
    /// <param name="id">Entry id</param>
    /// <returns>Database entry</returns>
    /// <remarks>
    /// This method fetches one entry from the database.
    /// </remarks>
    public static DatabaseEntry FetchOne(string id)
    {
      DatabaseEntry entry = database.Entries.Find(entry => entry.Id == id);
      entry.Identity = ResolveConstant(entry.Identity);
      return entry;
    }

    /// <summary>
    /// Query entries
    /// </summary>
    /// <param name="keyword">Search keyword</param>
    /// <returns>List of listable database entries</returns>
    /// <remarks>
    /// This method queries the database for entries matching the search keyword.
    /// </remarks>
    public static List<ListableDatabaseEntry> Query(string keyword) => database.Entries.Where(entry =>
        (entry.Platform != null && entry.Platform.Contains(keyword)) ||
        (entry.Identity != null && entry.Identity.Contains(keyword)) ||
        (entry.Url != null && entry.Url.Contains(keyword))
      ).Select(ConvertEntryToListable
      ).ToList();

    /// <summary>
    /// Update entry
    /// </summary>
    /// <param name="id">Entry id</param>
    /// <param name="entry">Database entry</param>
    /// <remarks>
    /// This method updates an entry in the database.
    /// </remarks>
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

    /// <summary>
    /// Delete entry
    /// </summary>
    /// <param name="id">Entry id</param>
    /// <remarks>
    /// This method deletes an entry from the database.
    /// </remarks>
    public static void Delete(string id)
    {
      database.Entries.RemoveAll(entry => entry.Id == id);
      SaveToFile();
    }

    /*
     * Constants pair methods
     */

    /// <summary>
    /// Declare constant pair
    /// </summary>
    /// <param name="entry">Constant pair entry</param>
    public static void DeclareConstant(ConstantPair entry)
    {
      Validate.ConstantPair(entry);
      database.Constants ??= [];
      database.Constants.Add(entry);
      SaveToFile();
    }

    /// <summary>
    /// Fetch constant pair
    /// </summary>
    /// <param name="constant">Constant key</param>
    /// <returns>Constant pair</returns>
    public static ConstantPair FetchConstant(string constant) =>
      (database.Constants ?? []).Find(pair =>
        pair.Key == constant
      );

    /// <summary>
    /// Forget constant pair
    /// </summary>
    /// <param name="constant">Constant key</param>
    public static void ForgetConstant(string constant)
    {
      if (FetchConstant(constant) == null) Error.EntryNotFound();
      database.Constants.RemoveAll((pair) =>
        pair.Key == constant
      );
      SaveToFile();
    }

    /// <summary>
    /// Get all constants
    /// </summary>
    /// <returns>List of constant pairs</returns>
    public static List<ConstantPair> AllConstants => database.Constants ?? [];

    /// <summary>
    /// Resolve constant
    /// </summary>
    /// <param name="constant">Constant key</param>
    /// <returns>Constant value</returns>
    public static string ResolveConstant(string key) =>
      AllConstants.Find((pair) =>
        $"_${pair.Key}" == key
      )?.Value ?? key;

    /*
     * Conversion methods
     */

    /// <summary>
    /// Get All Entries
    /// </summary>
    /// <returns>List of all entries</returns>
    /// <remarks>
    /// This method fetches all entries from the database. Only for statistics purposes.
    /// </remarks>
    public static DatabaseEntry[] AllEntries => [.. database.Entries];

    /// <summary>
    /// Convert entry to listable format
    /// </summary>
    /// <param name="entry">Database entry</param>
    /// <returns>Listable database entry</returns>
    /// <remarks>
    /// This method converts a database entry to a listable database entry.
    /// </remarks>
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

    /// <summary>
    /// Convert entry to countable format
    /// </summary>
    /// <param name="entry">Database entry</param>
    /// <returns>Countable database entry</returns>
    /// <remarks>
    /// This method converts a database entry to a countable database entry.
    /// </remarks>
    public static CountableDatabaseEntry ConvertEntryToCountable(DatabaseEntry entry) => new()
    {
      Platform = entry.Platform,
      Id = entry.Id,
      Url = entry.Url,
      TotalAccesses = entry.TotalAccesses
    };

    /// <summary> 
    /// Convert entry to micro format
    /// </summary>
    /// <param name="entry">Database entry</param>
    /// <returns>Micro database entry</returns>
    /// <remarks>
    /// This method converts a database entry to a micro database entry.
    /// </remarks>
    public static MicroDatabaseEntry ConvertEntryToMicro(DatabaseEntry entry) => new()
    {
      Platform = entry.Platform,
      Id = entry.Id,
      Url = entry.Url
    };
  }

  /// <summary>
  /// Database model for Passenger
  /// </summary>
  /// <remarks>
  /// This class defines the structure of the database.
  /// </remarks>
  public class DatabaseModel
  {

    /// <summary>
    /// Username to access the database
    /// </summary>
    /// <remarks>
    /// This should be the same as the
    /// username on keyring to be used
    /// to protect the secret key of 
    /// AES GCM encryption.
    /// Secret key will be gathered 
    /// from an environment variable.
    /// </remarks>
    [JsonPropertyName("username")]
    public string Username { get; set; }
    /// <summary>
    /// Passphrase to access the database
    /// </summary>
    [JsonPropertyName("passphrase")]
    public string Passphrase { get; set; }
    /// <summary>
    /// A list of passphrases in the database
    /// </summary>
    [JsonPropertyName("entries")]
    public List<DatabaseEntry> Entries { get; set; }
    /// <summary>
    /// A list of constants in the database
    /// </summary>
    [JsonPropertyName("constants")]
    public List<ConstantPair> Constants { get; set; }
  }

  /// <summary>
  /// Key-value pair for constants to use short names on the database
  /// </summary>
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

  /// <summary>
  /// Listable database entry for Passenger, does not include passphrase
  /// </summary>
  /// <remarks>
  /// This class defines the structure of a listable database entry.
  /// </remarks>
  public class ListableDatabaseEntry : CountableDatabaseEntry
  {
    [JsonPropertyName("identity"), Required]
    public string Identity { get; set; }
    [JsonPropertyName("created"), Required] // Auto-generated
    public string Created { get; set; }
    [JsonPropertyName("updated"), Required] // Auto-generated
    public string Updated { get; set; }
  }

  /// <summary>
  /// Database entry for Passenger
  /// </summary>
  /// <remarks>
  /// This class defines the structure of a database entry.
  /// </remarks>
  public class DatabaseEntry : ListableDatabaseEntry
  {
    [JsonPropertyName("passphrase"), Required]
    public string Passphrase { get; set; }
    [JsonPropertyName("notes")] // Optional
    public string Notes { get; set; }
  }

  /// <summary>
  /// Credentials Object
  /// </summary>
  /// <remarks>
  /// This class defines the structure of a credentials object.
  /// </remarks>
  public class Credentials
  {
    public string Username { get; set; }
    public string Passphrase { get; set; }
  }
}
