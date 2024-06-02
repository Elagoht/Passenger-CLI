namespace Passenger
{
  /// <summary>
  /// Main program
  /// </summary>
  /// <remarks>
  /// This class is responsible for handling command line arguments and executing the appropriate command.
  /// </remarks>
  public static class Program
  {
    /// <summary>
    /// Main entry point
    /// </summary>
    /// <param name="args"></param>
    /// <remarks>
    /// This method is responsible for parsing command line arguments and executing the appropriate command.
    /// </remarks>
    public static void Main(string[] args)
    {
      if (args.Length == 0) Error.MissingCommand();
      CommandLine commandLine = new(args);
      commandLine.Parse();
    }
  }
}
