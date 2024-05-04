namespace Passenger
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CommandLine commandLine = new(args);
      commandLine.Parse();
    }
  }
}
