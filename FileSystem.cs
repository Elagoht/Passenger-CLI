namespace Passenger
{
  /// <summary>
  /// Interact with file system
  /// </summary>
  /// <remarks>
  /// This class is responsible for reading and writing data to the file system.
  /// </remarks>
  public class FileSystem
  {
    /// <summary>
    /// Handle exceptions
    /// </summary>
    /// <param name="exception"></param>
    /// <remarks>
    /// This method is responsible for handling exceptions that may occur while accessing the file system. Exits with appropriate exit codes.
    /// </remarks>
    private static void Exceptions(Exception exception)
    {
      switch (exception)
      {
        case FileNotFoundException:
          Console.WriteLine("passenger: data file not found");
          Environment.Exit(1); break;
        case UnauthorizedAccessException:
          Console.WriteLine("passenger: data file access denied");
          Environment.Exit(126); break;
        case IOException:
          Console.WriteLine("passenger: data file input/output error");
          Environment.Exit(1); break;
        default:
          Console.WriteLine("passenger: unexpected error while accessing data file");
          Environment.Exit(2); break;
      }
    }

    /// <summary>
    /// Read data from file
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns>string</returns>
    /// <remarks>
    /// This method reads data from the file system and returns it as a string.
    /// </remarks>
    public static string Read(string fileName)
    {
      try
      {
        if (!File.Exists(fileName)) return "";
        return EnDeCoder.Decode(new StreamReader(fileName).ReadToEnd());
      }
      catch (Exception exception) { Exceptions(exception); throw; }
    }

    /// <summary>
    /// Write data to file
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    /// <remarks>
    /// This method writes data to the file system.
    /// </remarks>
    public static void Write(string fileName, string data)
    {
      try { File.WriteAllText(fileName, EnDeCoder.Encode(data)); }
      catch (Exception exception) { Exceptions(exception); }
    }
  }
}