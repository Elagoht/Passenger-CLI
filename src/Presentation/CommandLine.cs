namespace Passenger
{
  public class CommandLine(string[] args, string piped)
  {
    private readonly string command = args.Length > 0
      ? args[0]
      : null;

    public void Parse()
    {
      if (command is null) Error.MissingCommand();
      Worker worker = new(args, piped);
      switch (command)
      {
        case "login" or "-l": worker.Login(); break;
        case "register" or "-r": worker.Register(); break;
        case "reset": worker.Reset(); break;
        case "create" or "-c": worker.Create(); break;
        case "fetchAll" or "-a": worker.FetchAll(); break;
        case "fetch" or "-f": worker.Fetch(); break;
        case "query" or "-q": worker.Query(); break;
        case "update" or "-u": worker.Update(); break;
        case "delete" or "-d": worker.Delete(); break;
        case "stats" or "-s": worker.Statistics(); break;
        case "detect" or "-t": worker.Detect(); break;
        case "import" or "-i": worker.Import(); break;
        case "export" or "-e": worker.Export(); break;
        case "declare" or "-D": worker.Declare(); break;
        case "modify" or "-M": worker.Modify(); break;
        case "remember" or "-R": worker.Remember(); break;
        case "forget" or "-F": worker.Forget(); break;
        case "constants" or "-C": worker.Constants(); break;
        case "generate" or "-g": worker.Generate(); break;
        case "manipulate" or "-m": worker.Manipulate(); break;
        case "version" or "-v" or "--version": Worker.Version(); break;
        case "help" or "--help" or "-h": Worker.Help(); break;
        case "man": Worker.Manual(); break;
        default: Error.UnknownCommand(); break;
      }
    }
  }
}