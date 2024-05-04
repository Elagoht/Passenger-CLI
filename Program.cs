namespace Passenger
{
  public class Program
  {
    public static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine("passenger: missing command");
        Environment.Exit(1);
      }
      CommandLine commandLine = new(args);
      commandLine.Parse();
    }
  }
}
