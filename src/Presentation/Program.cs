namespace Passenger
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      CommandLine commandLine = new(args);
      commandLine.Parse();
    }
  }
}
