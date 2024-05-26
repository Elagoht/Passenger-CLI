namespace Passenger
{
  public class Statistics(DatabaseEntry[] entries)
  {
    private readonly DatabaseEntry[] entries = entries;

    public int TotalCount => entries.Length;

    public double AverageLength => entries.Select(entry =>
      entry.Passphrase.Length
    ).Sum() / TotalCount;

    public string[] UniquePlatforms => entries.Select(entry =>
      entry.Platform
    ).Distinct().ToArray();

    public int UniqueCount => entries.Select(entry =>
      entry.Passphrase
    ).Distinct().Count();

    public DatabaseEntry[] MostAccessed(int limit) => entries.OrderByDescending(entry =>
      entry.TotalAccesses
    ).Take(limit).ToArray();

    public DatabaseEntry[][] CommonByPlatform()
    {
      List<DatabaseEntry[]> commonPasswords = [];

      foreach (DatabaseEntry entry in entries)
      {
        DatabaseEntry[] commonPassword = entries.Where(e =>
          e.Passphrase == entry.Passphrase
        ).ToArray();

        if (commonPassword.Length > 1)
          commonPasswords.Add(commonPassword);
      }

      return [.. commonPasswords];
    }

    public double PercentageOfCommon => CommonByPlatform().Length / TotalCount * 100;

    public string MostCommon => CommonByPlatform().OrderByDescending(commonPasswords =>
      commonPasswords.Length
    ).First()[0].Passphrase;

    public Dictionary<string, int> Strengths => entries.ToDictionary(entry =>
      entry.Id,
      entry => Strength.Calculate(entry.Passphrase)
    );

    public double AverageStrength => Strengths.Values.Average();

    public DatabaseEntry[] WeakPassphrases => entries.Where(entry =>
      Strengths[entry.Id] < 4
    ).ToArray();

    public DatabaseEntry[] MediumPassphrases => entries.Where(entry =>
      Strengths[entry.Id] >= 4 && Strengths[entry.Id] <= 5
    ).ToArray();

    public DatabaseEntry[] StrongPassphrases => entries.Where(entry =>
      Strengths[entry.Id] > 5
    ).ToArray();
  }
}