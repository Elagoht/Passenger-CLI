using System.Buffers.Text;
using System.Text;

namespace Passenger
{
  public static class Mapper
  {
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

    public static string ToCSVLine(ReadWritableDatabaseEntry entry) =>
      $"{entry.Platform},{entry.Url},{entry.Identity},{entry.Passphrase},{entry.Notes}";

    public static string ToEncryptedCSVLine(ReadWritableDatabaseEntry entry) =>
      Convert.ToBase64String(Encoding.UTF8.GetBytes(
        ToCSVLine(entry)
      ));

    public static DatabaseEntry CreateDatabaseEntry(string platform, string identity, string url, string passphrase, string notes = null) =>
      new()
      {
        Id = Guid.NewGuid().ToString(),
        Platform = platform,
        Identity = identity,
        Url = url,
        Notes = notes,
        Passphrase = passphrase,
        PassphraseHistory = [new() {
          Length = passphrase.Length,
          Strength = Strength.Calculate(passphrase),
          Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        }],
        Created = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        Updated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
        TotalAccesses = 0,
      };
  }
}