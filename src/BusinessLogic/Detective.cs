namespace Passenger
{
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
