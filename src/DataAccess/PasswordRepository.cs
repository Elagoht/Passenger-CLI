using System.Reflection;

namespace Passenger
{
  public static class PasswordRepository
  {
    public static List<string> Load(int length)
    {
      List<string> passwordList = [];
      string resourceName = $"Passenger.resources.passwords_{length}.txt";

      using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
      {
        if (stream == null) return passwordList;
        using StreamReader reader = new(stream);
        string line;
        while ((line = reader.ReadLine()) != null)
          passwordList.Add(line);
      }
      return passwordList;
    }

    public static int BinarySearch(List<string> sortedList, string target)
    {
      int left = 0, right = sortedList.Count - 1;
      while (left <= right)
      {
        int mid = left + (right - left) / 2;
        int comparison = string.Compare(
          sortedList[mid], target,
          StringComparison.Ordinal
        );

        if (comparison == 0) return mid;
        if (comparison < 0) left = mid + 1;
        else right = mid - 1;
      }
      return -1;
    }
  }
}