using System.Text.Json.Serialization;

namespace Passenger
{
  public class Statistics(DatabaseEntry[] entries)
  {
    private readonly DatabaseEntry[] entries = entries;

    public int TotalCount => entries.Length;

    public double AverageLength => TotalCount == 0
      ? 0
      : entries.Select(entry =>
        entry.Passphrase.Length
      ).Sum() / TotalCount;

    public string[] UniquePlatforms => entries.Select(entry =>
      entry.Platform
    ).Distinct().ToArray();

    public int UniquePlatformsCount => UniquePlatforms.Length;

    public int UniquePassphrases => entries.Select(entry =>
      entry.Passphrase
    ).Distinct().Count();

    public CountableDatabaseEntry[] MostAccessed => entries.OrderByDescending(
      entry => entry.TotalAccesses
    ).Take(5
    ).Select(Database.ConvertEntryToCountable
    ).ToArray();

    public MicroDatabaseEntry[][] CommonByPlatform => entries.Select(entry =>
    {
      MicroDatabaseEntry[] commonPassword = entries.Where(current =>
        current.Passphrase == entry.Passphrase
      ).OrderBy(entry => entry.Platform
      ).Select(Database.ConvertEntryToMicro
      ).ToArray();

      return commonPassword.Length > 1
        ? commonPassword
        : null;
    }).Where(commonPassword =>
      commonPassword != null
    ).ToArray();

    public double PercentageOfCommon => TotalCount == 0
      ? 0
      : CommonByPlatform.SelectMany(commonPassword =>
        commonPassword
      ).Distinct().Count() / TotalCount;

    public string MostCommon => TotalCount == 0
      ? "Not available"
      : entries.GroupBy(entry =>
        entry.Passphrase
      ).OrderByDescending(group =>
        group.Count()
      ).First().Key;

    public Dictionary<string, int> Strengths => entries.ToDictionary(entry =>
      entry.Id,
      entry => Strength.Calculate(entry.Passphrase)
    );

    public double AverageStrength => TotalCount == 0
      ? -2
      : Strengths.Values.Average();

    public MicroDatabaseEntry[] WeakPassphrases => [..entries.Where(entry =>
      Strengths[entry.Id] < 4
    ).Select(Database.ConvertEntryToMicro)];

    public MicroDatabaseEntry[] MediumPassphrases => [..entries.Where(entry =>
      Strengths[entry.Id] >= 4 && Strengths[entry.Id] <= 5
    ).Select(Database.ConvertEntryToMicro)];

    public MicroDatabaseEntry[] StrongPassphrases => [..entries.Where(entry =>
      Strengths[entry.Id] > 5
    ).Select(Database.ConvertEntryToMicro)];
  }

  public class DashboardData
  {
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
    [JsonPropertyName("averageLength")]
    public double AverageLength { get; set; }
    [JsonPropertyName("uniquePlatforms")]
    public string[] UniquePlatforms { get; set; }
    [JsonPropertyName("uniquePlatformsCount")]
    public int UniquePlatformsCount { get; set; }
    [JsonPropertyName("uniquePassphrases")]
    public int UniquePassphrases { get; set; }
    [JsonPropertyName("mostAccessed")]
    public CountableDatabaseEntry[] MostAccessed { get; set; }
    [JsonPropertyName("commonByPlatform")]
    public MicroDatabaseEntry[][] CommonByPlatform { get; set; }
    [JsonPropertyName("percentageOfCommon")]
    public double PercentageOfCommon { get; set; }
    [JsonPropertyName("mostCommon")]
    public string MostCommon { get; set; }
    [JsonPropertyName("strengths")]
    public Dictionary<string, int> Strengths { get; set; }
    [JsonPropertyName("averageStrength")]
    public double AverageStrength { get; set; }
    [JsonPropertyName("weakPassphrases")]
    public MicroDatabaseEntry[] WeakPassphrases { get; set; }
    [JsonPropertyName("mediumPassphrases")]
    public MicroDatabaseEntry[] MediumPassphrases { get; set; }
    [JsonPropertyName("strongPassphrases")]
    public MicroDatabaseEntry[] StrongPassphrases { get; set; }
  }
}