namespace Passenger
{
  public class Detective
  {
    private readonly List<DatabaseEntry> entries;

    public List<List<DatabaseEntry>> CommonPasswords { get; private set; }
    public List<DatabaseEntry> SimilarWithUsername { get; private set; }
    public List<DatabaseEntry> WeakPasswords { get; private set; }
    public List<DatabaseEntry> OldPasswords { get; private set; }

    public Detective(List<DatabaseEntry> entries)
    {
      this.entries = entries;
      CommonPasswords = SetCommonPasswords();
      SimilarWithUsername = SetSimilarWithUsername();
    }

    public List<List<DatabaseEntry>> SetCommonPasswords() => entries
      .GroupBy(entry => entry.Passphrase)
      .Where(group => group.Count() > 1)
      .Select(group => group.ToList())
      .ToList();

    public List<DatabaseEntry> SetSimilarWithUsername() => entries
      .Where(Investigator.IsPasswordSimilarToUsername)
      .ToList();

    public List<DatabaseEntry> SetWeakPasswords() => entries
      .Where(entry => Strength.Calculate(entry.Passphrase) < 4)
      .ToList();

    public List<DatabaseEntry> SetOldPasswords() => entries
      .Where(entry => DateTime
        .Parse(
          entry.PassphraseHistory.Last().Created
        ) < DateTime.Now.AddYears(-1)
      ).ToList();
  }

  public static class Investigator
  {
    public static bool IsPasswordSimilarToUsername(DatabaseEntry entry) =>
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
