namespace Passenger
{
  public static class FileSystem
  {
    public static string Read(string fileName)
    {
      try
      {
        if (!File.Exists(fileName)) return "";
        return EnDeCoder.Decode(new StreamReader(fileName).ReadToEnd());
      }
      catch (Exception exception) { Error.FileExceptions(exception); throw; }
    }

    public static void Write(string fileName, string data)
    {
      try { File.WriteAllText(fileName, EnDeCoder.Encode(data)); }
      catch (Exception exception) { Error.FileExceptions(exception); }
    }
  }
}