using System.Text.Json;

namespace Passenger
{
  public static class Validate
  {
    public static DatabaseEntry Entry(DatabaseEntry entry)
    {
      // Check if required fields are provided
      if (string.IsNullOrEmpty(entry.Platform)) Error.MissingField("platform");
      if (string.IsNullOrEmpty(entry.Passphrase)) Error.MissingField("passphrase");
      if (string.IsNullOrEmpty(entry.Url)) Error.MissingField("url");
      if (string.IsNullOrEmpty(entry.Identity)) Error.MissingField("identity");
      return entry;
    }

    public static ConstantPair ConstantPair(ConstantPair entry)
    {
      // Check if required fields are provided
      if (string.IsNullOrEmpty(entry.Key)) Error.MissingField("key");
      if (string.IsNullOrEmpty(entry.Value)) Error.MissingField("value");
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