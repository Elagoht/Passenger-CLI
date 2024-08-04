using System.Text.Json;

namespace Passenger
{
  public static class Validate
  {
    public static void Entry(ReadWritableDatabaseEntry databaseEntry)
    {
      // Check if required fields are provided
      if (string.IsNullOrEmpty(databaseEntry.Platform)) Error.MissingField("platform");
      if (string.IsNullOrEmpty(databaseEntry.Passphrase)) Error.MissingField("passphrase");
      if (string.IsNullOrEmpty(databaseEntry.Url)) Error.MissingField("url");
      if (string.IsNullOrEmpty(databaseEntry.Identity)) Error.MissingField("identity");
      PassphraseLength(databaseEntry.Passphrase);
      IfIsOnRepository(databaseEntry.Passphrase);
    }

    public static bool EntryFields(ReadWritableDatabaseEntry databaseEntry) =>
      databaseEntry switch
      {
        { Platform: null } => false,
        { Passphrase: null } => false,
        { Passphrase.Length: < 8 or > 4096 } => false,
        { Url: null } => false,
        { Identity: null } => false,
        _ => true
      };

    public static void ConstantPair(ConstantPair entry)
    {
      // Check if required fields are provided
      if (string.IsNullOrEmpty(entry.Key)) Error.MissingField("key");
      if (string.IsNullOrEmpty(entry.Value)) Error.MissingField("value");
    }

    public static ReadWritableDatabaseEntry JsonAsDatabaseEntry(string json)
    {
      try { return JsonSerializer.Deserialize<ReadWritableDatabaseEntry>(json); }
      catch { Error.JsonParseError(); return null; }
    }

    public static void PassphraseLength(string passphrase)
    {
      if (passphrase.Length < 8) Error.PassphraseTooShort();
      if (passphrase.Length > 4096) Error.PassphraseTooLong();
    }

    public static void IfIsOnRepository(string passphrase)
    {
      // There is no password longer than on brute force list
      if (passphrase.Length > 285) return;
      if (
        PasswordRepository.BinarySearch(
          PasswordRepository.Load(passphrase.Length),
          passphrase
        ) >= 0
      ) Error.FoundOnRepository();
    }
  }
}