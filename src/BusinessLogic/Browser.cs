using CsvHelper.Configuration;

namespace Passenger
{
  class Browser
  {
    public const string Chromium = "chromium";
    public const string Firefox = "firefox";
    public const string Safari = "safari";

    public const string Bare = "bare";
    public const string Encrypted = "encrypted";

    public static readonly string[] SupportedBrowsers = [Chromium, Firefox, Safari];
    public static readonly string[] exportTypes = [Bare, Encrypted];

    public static List<DatabaseEntry> Import(string browser, string content) =>
      browser switch
      {
        Chromium => ImportData.FromChromium(content),
        Firefox => ImportData.FromFirefox(content),
        Safari => ImportData.FromSafari(content),
        _ => throw new ArgumentException("Invalid browser type")
      };

    public static string Export(string exportType, List<ReadWritableDatabaseEntry> entries) =>
      exportType switch
      {
        Bare => ExportData.ToBare(entries),
        Encrypted => ExportData.ToEncrypted(entries),
        _ => throw new ArgumentException("Invalid export type")
      };

    internal class ImportData
    {
      internal static List<DatabaseEntry> FromChromium(string content)
      {
        List<ChromiumFields> records = CSV.ReadTyped<ChromiumFields, ChromiumMap>(content);
        List<DatabaseEntry> mappedEntries = [];
        foreach (ChromiumFields record in records)
          mappedEntries.Add(Mapper.CreateDatabaseEntry(
            record.Name, record.Username, record.Url,
            record.Password, record.Note
          ));
        return mappedEntries;
      }

      internal static List<DatabaseEntry> FromFirefox(string content)
      {
        List<FirefoxFields> records = CSV.ReadTyped<FirefoxFields, FirefoxMap>(content);
        List<DatabaseEntry> mappedEntries = [];
        foreach (FirefoxFields record in records)
          mappedEntries.Add(Mapper.CreateDatabaseEntry(
            ConvertURLToPlatformName(record.Url),
            record.Username, record.Url, record.Password
          ));
        return mappedEntries;
      }

      internal static List<DatabaseEntry> FromSafari(string content)
      {
        List<SafariFields> records = CSV.ReadTyped<SafariFields, SafariMap>(content);
        List<DatabaseEntry> mappedEntries = [];
        foreach (SafariFields record in records)
          mappedEntries.Add(Mapper.CreateDatabaseEntry(
            NormalizeApplePlatformName(record.Title),
            record.Username, record.Url,
            record.Password, record.Notes
          ));
        return mappedEntries;
      }

      internal static string ConvertURLToPlatformName(string url)
      {
        string host = new Uri(url).Host.ToLower();
        string[] parts = host.Split('.');
        if (parts.Length > 2 && parts[^1].Length == 2)
          host = parts[^3];
        else if (parts.Length >= 2)
          host = parts[^2];
        return host[0].ToString().ToUpper() + host[1..];
      }

      internal static string NormalizeApplePlatformName(string platform) =>
        ConvertURLToPlatformName($"https://{string.Join(
          " ",
          platform.Split(" (")[..^1]
        )}");

      internal class ChromiumFields
      {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Note { get; set; }
      }

      internal class FirefoxFields
      {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FormActionOrigin { get; set; }
      }

      internal class SafariFields
      {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
      }

      internal class ChromiumMap : ClassMap<ChromiumFields>
      {
        public ChromiumMap()
        {
          Map(model => model.Name).Name("name");
          Map(model => model.Url).Name("url");
          Map(model => model.Username).Name("username");
          Map(model => model.Password).Name("password");
          Map(model => model.Note).Name("note");
        }
      }

      internal class FirefoxMap : ClassMap<FirefoxFields>
      {
        public FirefoxMap()
        {
          Map(model => model.Url).Name("url");
          Map(model => model.Username).Name("username");
          Map(model => model.Password).Name("password");
          Map(model => model.FormActionOrigin).Name("formActionOrigin");
        }
      }

      internal class SafariMap : ClassMap<SafariFields>
      {
        public SafariMap()
        {
          Map(model => model.Title).Name("Title");
          Map(model => model.Url).Name("URL");
          Map(model => model.Username).Name("Username");
          Map(model => model.Password).Name("Password");
          Map(model => model.Notes).Name("Notes");
        }
      }
    }

    internal class ExportData
    {
      internal static string ToBare(List<ReadWritableDatabaseEntry> entries) =>
        "name,url,username,password,note\n" +
        string.Join("\n", entries.Select(Mapper.ToCSVLine));

      internal static string ToEncrypted(List<ReadWritableDatabaseEntry> entries) =>
        "bmFtZSx1cmwsdXNlcm5hbWUscGFzc3dvcmQsbm90ZQ==\n" +
        string.Join("\n", entries.Select(Mapper.ToEncryptedCSVLine));
    }
  }
}