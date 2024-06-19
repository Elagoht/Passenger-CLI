namespace Passenger
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      string piped = null;
      // Get piped input if exists
      if (Console.IsInputRedirected)
      {
        using Stream inputStream = Console.OpenStandardInput();
        using StreamReader streamReader = new(inputStream);
        piped = streamReader.ReadToEnd().Trim();
      }
      CommandLine commandLine = new(args, piped);
      commandLine.Parse();
    }
  }
}
