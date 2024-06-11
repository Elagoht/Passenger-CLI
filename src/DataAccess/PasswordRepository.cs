using System.Reflection;

namespace Passenger
{
  public class PasswordRepository
  {
    public static List<string> Load()
    {
      var passwordList = new List<string>();
      var assembly = Assembly.GetExecutingAssembly();
      using (Stream stream = assembly.GetManifestResourceStream("Passenger.passwords.bin"))
      {
        if (stream != null)
        {
          using var binaryReader = new BinaryReader(stream);
          int count = binaryReader.ReadInt32();
          for (int cursor = 0; cursor < count; cursor++)
            passwordList.Add(binaryReader.ReadString());
        }
        else throw new Exception("Resource not found.");
      }
      return passwordList;
    }
  }
}