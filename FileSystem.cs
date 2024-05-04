namespace Passenger
{
  public class File
  {
    private static void Exceptions(Exception exception)
    {
      if (exception is FileNotFoundException)
      {
        Console.WriteLine("passenger: data file not found");
        Environment.Exit(1);
      }
      else if (exception is UnauthorizedAccessException)
      {
        Console.WriteLine("passenger: data file access denied");
        Environment.Exit(126);
      }
      else if (exception is IOException)
      {
        Console.WriteLine("passenger: data file input/output error");
        Environment.Exit(1);
      }
      else
      {
        Console.WriteLine("passenger: unexpected error while accessing data file");
        Environment.Exit(2);
      }
    }

    public static string Read(string fileName)
    {
      try
      {
        using StreamReader reader = new(fileName);
        return reader.ReadToEnd();
      }
      catch (Exception exception)
      {
        Exceptions(exception);
        throw;
      }
    }

    public static void Write(string fileName, string data)
    {
      try
      {
        using StreamWriter writer = new(fileName);
        writer.Write(data);
      }
      catch (Exception exception)
      {
        Exceptions(exception);
      }
    }
  }
}