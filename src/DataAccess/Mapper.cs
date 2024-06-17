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
      Passphrase = entry.Passphrase,
      PassphraseHistory = [
        new() {
          Created = entry.Created,
          Length= entry.Passphrase.Length,
          Strength = Strength.Calculate(entry.Passphrase)
        }
      ],
      Notes = entry.Notes
    };

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
  }
}