using System.Reflection;

namespace Passenger
{
  public static class PasswordRepository
  {
    public static List<string> Load(int length)
    {
      List<string> passwordList = [];

      using (Stream stream = Assembly
          .GetExecutingAssembly()
          .GetManifestResourceStream($"Passenger.resources.passwords_{length}.bin"))
      {
        if (stream == null) return passwordList;
        using var binaryReader = new BinaryReader(stream);
        int count = binaryReader.ReadInt32();
        for (int cursor = 0; cursor < count; ++cursor)
        {
          int passwordLength = binaryReader.ReadInt32();
          byte[] passwordBytes = binaryReader.ReadBytes(passwordLength);
          passwordList.Add(System.Text.Encoding.UTF8.GetString(passwordBytes));
        }
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