namespace Passenger
{
  public static class Mapper
  {
    public static DatabaseEntry NewlyCreated(ReadWritableDatabaseEntry entry) => new()
    {
      Id = entry.Id,
      Platform = entry.Platform,
      Url = entry.Url,
      Identity = entry.Identity,
      Created = entry.Created,
      Updated = entry.Updated,
      TotalAccesses = entry.TotalAccesses,
      Passphrases = [new() { Passphrase = entry.Passphrase, Updated = entry.Updated }],
      Notes = entry.Notes
    };

    public static List<TrackablePassphrase> RegisterNewPassphrase(
      List<TrackablePassphrase> passphrases,
      string passphrase
    ) => [
      .. passphrases,
      new() {
        Passphrase = passphrase,
        Updated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
      }
    ];

    public static ReadWritableDatabaseEntry ToReadWritable(DatabaseEntry entry) => new()
    {
      Id = entry.Id,
      Platform = entry.Platform,
      // Get the last set passphrase
      Passphrase = entry.Passphrases.Last().Passphrase,
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
  }
}