namespace Passenger
{
  public static class OperatingSystem
  {
    public enum OSType
    {
      Unix,
      MacOS,
      Windows,
      Unknown
    }

    public static bool IsUnix => Environment.OSVersion.Platform == PlatformID.Unix;
    public static bool IsMacOS => Environment.OSVersion.Platform == PlatformID.MacOSX;
    public static bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT;

    public static OSType CurrentOS =>
      IsUnix ? OSType.Unix
      : IsMacOS ? OSType.MacOS
      : IsWindows ? OSType.Windows
      : OSType.Unknown;

    public static string HomeDirectory => CurrentOS switch
    {
      OSType.Unix or OSType.MacOS => Environment.GetEnvironmentVariable("HOME")
        ?? throw new InvalidOperationException("HOME environment variable is not set"),
      OSType.Windows => Environment.GetEnvironmentVariable("USERPROFILE")
        ?? throw new InvalidOperationException("USERPROFILE environment variable is not set"),
      _ => throw new PlatformNotSupportedException()
    };

    public static string StoragePath => Path.Combine(
      HomeDirectory,
      CurrentOS == OSType.Windows
        ? Path.Combine("AppData", "Roaming", ".passenger", "passenger.bus")
        : Path.Combine(".passenger", "passenger.bus")
    );
  }
}
