using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Passenger
{
  public class Detective
  {
    private readonly List<DatabaseEntry> entries;

    [JsonPropertyName("commonPassphrases"), Required]
    public List<List<ListableDatabaseEntry>> CommonPassphrases { get; private set; }
    [JsonPropertyName("similarWithUsername"), Required]
    public List<ListableDatabaseEntry> SimilarWithUsername { get; private set; }
    [JsonPropertyName("weakPassphrases"), Required]
    public List<ListableDatabaseEntry> WeakPassphrases { get; private set; }
    [JsonPropertyName("oldPassphrases"), Required]
    public List<ListableDatabaseEntry> OldPassphrases { get; private set; }

    public Detective(List<DatabaseEntry> entries)
    {
      this.entries = entries;
      CommonPassphrases = SetCommonPassphrases();
      SimilarWithUsername = SetSimilarWithUsername();
      WeakPassphrases = SetWeakPassphrases();
      OldPassphrases = SetOldPassphrases();
    }

    private List<List<ListableDatabaseEntry>> SetCommonPassphrases() => entries
      .GroupBy(entry => entry.Passphrase)
      .Where(group => group.Count() > 1)
      .Select(group => group.Select(
          Mapper.ToListable
        ).ToList()
      ).ToList();

    private List<ListableDatabaseEntry> SetSimilarWithUsername() => entries
      .Where(Investigator.IsPassphraseSimilarToUsername)
      .Select(Mapper.ToListable)
      .ToList();

    private List<ListableDatabaseEntry> SetWeakPassphrases() => entries
      .Where(entry => Strength.Calculate(entry.Passphrase) < 4)
      .Select(Mapper.ToListable)
      .ToList();

    private List<ListableDatabaseEntry> SetOldPassphrases() => entries
      .Where(entry => DateTime
        .Parse(
          entry.PassphraseHistory.Last().Created
        ) < DateTime.Now.AddYears(-1)
      ).Select(Mapper.ToListable
      ).ToList();
  }

  public static class Investigator
  {
    public static bool IsPassphraseSimilarToUsername(DatabaseEntry entry) =>
      ComputeLevenshteinDistance(
        entry.Identity.ToLower(),
        entry.Passphrase.ToLower()
      ) <= 3; // Threshold

    private static int ComputeLevenshteinDistance(string source, string target)
    {
      int sourceLength = source.Length;
      int targetLength = target.Length;

      int[,] distance = new int[sourceLength + 1, targetLength + 1];

      for (int currentIndex = 0; currentIndex <= sourceLength; currentIndex++)
        distance[currentIndex, 0] = currentIndex;

      for (int targetIndex = 0; targetIndex <= targetLength; targetIndex++)
        distance[0, targetIndex] = targetIndex;

      for (int currentIndex = 1; currentIndex <= sourceLength; currentIndex++)
        for (int targetIndex = 1; targetIndex <= targetLength; targetIndex++)
          distance[currentIndex, targetIndex] = Math.Min(
            Math.Min(
              distance[currentIndex - 1, targetIndex] + 1,
              distance[currentIndex, targetIndex - 1] + 1
            ),
            distance[currentIndex - 1, targetIndex - 1] + (
              source[currentIndex - 1] == target[targetIndex - 1]
                ? 0
                : 1
            )
          );

      return distance[sourceLength, targetLength];
    }
  }
}
