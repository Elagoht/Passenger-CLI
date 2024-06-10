using System.Text.Json;

namespace Passenger
{
  public static class Validate
  {
    public static DatabaseEntry Entry(DatabaseEntry databaseEntry)
    {
      // Check if required fields are provided
      if (string.IsNullOrEmpty(databaseEntry.Platform)) Error.MissingField("platform");
      if (string.IsNullOrEmpty(databaseEntry.Passphrase)) Error.MissingField("passphrase");
      if (string.IsNullOrEmpty(databaseEntry.Url)) Error.MissingField("url");
      if (string.IsNullOrEmpty(databaseEntry.Identity)) Error.MissingField("identity");
      return databaseEntry;
    }

    public static ConstantPair ConstantPair(ConstantPair entry, bool checkExistence = true)
    {
      // Check if required fields are provided
      if (string.IsNullOrEmpty(entry.Key)) Error.MissingField("key");
      if (string.IsNullOrEmpty(entry.Value)) Error.MissingField("value");
      if (!checkExistence) return entry;
      if (Database.FetchConstant(entry.Key) != null) Error.ConstantExists(entry);
      return entry;
    }

    public static DatabaseEntry JsonAsDatabaseEntry(string json)
    {
      try { return JsonSerializer.Deserialize<DatabaseEntry>(json); }
      catch { Error.JsonParseError(); return null; }
    }
  }
}