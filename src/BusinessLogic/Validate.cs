using System.Text.Json;

namespace Passenger
{

  /// <summary>
  /// Validation methods for Passenger
  /// </summary>
  /// <remarks>
  /// This class provides validation methods for database entries.
  /// </remarks>
  public static class Validate
  {
    /// <summary>
    /// Validate entry
    /// </summary>
    /// <param name="entry">Database entry</param>
    /// <returns>Validated database entry</returns>
    /// <remarks>
    /// This method validates a database entry and auto-generates created and updated fields.
    /// </remarks>
    public static DatabaseEntry Entry(DatabaseEntry entry)
    {
      // Check if required fields are provided
      if (string.IsNullOrEmpty(entry.Platform)) Error.MissingField("platform");
      if (string.IsNullOrEmpty(entry.Passphrase)) Error.MissingField("passphrase");
      if (string.IsNullOrEmpty(entry.Url)) Error.MissingField("url");
      if (string.IsNullOrEmpty(entry.Identity)) Error.MissingField("identity");
      return entry;
    }

    /// <summary>
    /// Validate Constant entry
    /// </summary>
    /// <param name="entry">Constant pair entry</param>
    /// <returns>Validated constant pair entry</returns>
    public static ConstantPair ConstantPair(ConstantPair entry)
    {
      // Check if required fields are provided
      if (string.IsNullOrEmpty(entry.Key)) Error.MissingField("key");
      if (string.IsNullOrEmpty(entry.Value)) Error.MissingField("value");
      if (Database.FetchConstant(entry.Key) != null) Error.ConstantExists(entry);
      return entry;
    }

    /// <summary>
    /// Validate JSON as database entry
    /// </summary>
    /// <param name="json">JSON string</param>
    /// <remarks>
    /// Validates and parses a JSON string as a database entry else exits the program with an error.
    /// </remarks>
    public static DatabaseEntry JsonAsDatabaseEntry(string json)
    {
      try { return JsonSerializer.Deserialize<DatabaseEntry>(json); }
      catch { Error.JsonParseError(); return null; }
    }
  }
}