using System.Reflection;

namespace Passenger
{
  public static class PasswordRepository
  {
    public static List<string> Load(ushort length)
    {
      List<string> passwordList = [];

      using (Stream stream = Assembly
        .GetExecutingAssembly()
        .GetManifestResourceStream($"Passenger.passwords_{length}.bin"))
      {
        if (stream == null) throw new Exception("Resource not found.");
        using var binaryReader = new BinaryReader(stream);
        int count = binaryReader.ReadInt32();
        for (int cursor = 0; cursor < count; cursor++)
          passwordList.Add(binaryReader.ReadString());
      }
      return passwordList;
    }
  }
}