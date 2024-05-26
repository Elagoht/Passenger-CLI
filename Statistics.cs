using System.Text.Json.Serialization;

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

    public int UniquePlatformsCount => UniquePlatforms.Length;

    public int UniquePassphrases => entries.Select(entry =>
      entry.Passphrase
    ).Distinct().Count();

    public ListableDatabaseEntry[] MostAccessed(int limit) => entries.OrderByDescending(entry =>
      entry.TotalAccesses
    ).Take(limit).ToArray();

    public ListableDatabaseEntry[][] CommonByPlatform()
    {
      List<ListableDatabaseEntry[]> commonPasswords = [];

      foreach (DatabaseEntry entry in entries)
      {
        ListableDatabaseEntry[] commonPassword = entries.Where(entry =>
          entry.Passphrase == entry.Passphrase
        ).OrderBy(entry => entry.Platform
        ).Select(Database.ConvertEntryToListable
        ).ToArray();

        if (commonPassword.Length > 1) commonPasswords.Add(commonPassword);
      }

      return [.. commonPasswords];
    }

    public double PercentageOfCommon => entries.GroupBy(entry =>
      entry.Passphrase
    ).Count(group =>
      group.Count() > 1
    ) / TotalCount;

    public string MostCommon => entries.GroupBy(entry =>
      entry.Passphrase
    ).OrderByDescending(group =>
      group.Count()
    ).First().Key;

    public Dictionary<string, int> Strengths => entries.ToDictionary(entry =>
      entry.Id,
      entry => Strength.Calculate(entry.Passphrase)
    );

    public double AverageStrength => Strengths.Values.Average();

    public ListableDatabaseEntry[] WeakPassphrases => [..entries.Where(entry =>
      Strengths[entry.Id] < 4
    ).Select(Database.ConvertEntryToListable)];

    public ListableDatabaseEntry[] MediumPassphrases => [..entries.Where(entry =>
      Strengths[entry.Id] >= 4 && Strengths[entry.Id] <= 5
    ).Select(Database.ConvertEntryToListable)];

    public ListableDatabaseEntry[] StrongPassphrases => [..entries.Where(entry =>
      Strengths[entry.Id] > 5
    ).Select(Database.ConvertEntryToListable)];
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
    public ListableDatabaseEntry[] MostAccessed { get; set; }
    [JsonPropertyName("commonByPlatform")]
    public ListableDatabaseEntry[][] CommonByPlatform { get; set; }
    [JsonPropertyName("percentageOfCommon")]
    public double PercentageOfCommon { get; set; }
    [JsonPropertyName("mostCommon")]
    public string MostCommon { get; set; }
    [JsonPropertyName("strengths")]
    public Dictionary<string, int> Strengths { get; set; }
    [JsonPropertyName("averageStrength")]
    public double AverageStrength { get; set; }
    [JsonPropertyName("weakPassphrases")]
    public ListableDatabaseEntry[] WeakPassphrases { get; set; }
    [JsonPropertyName("mediumPassphrases")]
    public ListableDatabaseEntry[] MediumPassphrases { get; set; }
    [JsonPropertyName("strongPassphrases")]
    public ListableDatabaseEntry[] StrongPassphrases { get; set; }
  }
}