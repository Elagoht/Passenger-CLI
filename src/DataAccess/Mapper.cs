namespace Passenger
{
  public static class Mapper
  {
    public static DatabaseEntry AutoRegistered(ReadWritableDatabaseEntry entry) =>
      CreateDatabaseEntry(
        entry.Platform, entry.Identity, entry.Url, entry.Passphrase, entry.Notes
      );

    public static ReadWritableDatabaseEntry ToReadWritable(DatabaseEntry entry) => new()
    {
      Id = entry.Id,
      Platform = entry.Platform,
      Passphrase = entry.Passphrase,
      Url = entry.Url,
      Identity = entry.Identity,
      Created = entry.Created,
      Updated = entry.Updated,
      TotalAccesses = entry.TotalAccesses
    };

    public static ListableDatabaseEntry ToListable(ListableDatabaseEntry entry) => new()
    {
      Id = entry.Id,
      Platform = entry.Platform,
      Identity = ResolveConstant(entry.Identity),
      Url = entry.Url,
      Created = entry.Created,
      Updated = entry.Updated,
      TotalAccesses = entry.TotalAccesses
    };

    public static CountableDatabaseEntry ToCountable(CountableDatabaseEntry entry) => new()
    {
      Platform = entry.Platform,
      Id = entry.Id,
      Url = entry.Url,
      TotalAccesses = entry.TotalAccesses
    };

    public static MicroDatabaseEntry ToMicro(MicroDatabaseEntry entry) => new()
    {
      Platform = entry.Platform,
      Id = entry.Id,
      Url = entry.Url
    };

    public static string ResolveConstant(string key) =>
      Database.AllConstants.Find((pair) =>
        $"_${pair.Key}" == key
      )?.Value ?? key;

    public static DatabaseEntry CreateDatabaseEntry(string platform, string username, string url, string password, string notes = null)
    {
      return new DatabaseEntry
      {
        Id = Guid.NewGuid().ToString(),
        Platform = platform,
        Identity = username,
        Url = url,
        Notes = notes,
        Passphrase = password,
        PassphraseHistory = [new() {
          Length = password.Length,
          Strength = Strength.Calculate(password),
          Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        }],
        Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        Updated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        TotalAccesses = 0,
      };
    }
  }
}