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

    public static List<DatabaseEntry>[] Import(string browser, string content) =>
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

    public class ImportData
    {
      public static List<DatabaseEntry>[] FromChromium(string content) =>
        ProcessRecords(
          CSV.ReadTyped<ChromiumFields, ChromiumMap>(content),
          (record) => Mapper.CreateDatabaseEntry(
            record.Name, record.Username, record.Url,
            record.Password, record.Note
          )
        );


      public static List<DatabaseEntry>[] FromFirefox(string content) =>
        ProcessRecords(
          CSV.ReadTyped<FirefoxFields, FirefoxMap>(content),
          (record) => Mapper.CreateDatabaseEntry(
            ConvertURLToPlatformName(record.Url),
            record.Username, record.Url, record.Password
          )
        );

      public static List<DatabaseEntry>[] FromSafari(string content) =>
        ProcessRecords(
          CSV.ReadTyped<SafariFields, SafariMap>(content),
          (record) => Mapper.CreateDatabaseEntry(
            NormalizeApplePlatformName(record.Title),
            record.Username, record.Url,
            record.Password, record.Notes
          )
        );

      private static List<DatabaseEntry>[] ProcessRecords<T>(List<T> records, Func<T, DatabaseEntry> createEntryFunc)
      {
        List<DatabaseEntry> mappedEntries = [];
        List<DatabaseEntry> skippedEntries = [];
        foreach (T record in records)
        {
          DatabaseEntry created = createEntryFunc(record);
          if (Validate.EntryFields(created))
            mappedEntries.Add(created);
          else
            skippedEntries.Add(created);
        }
        return [mappedEntries, skippedEntries];
      }

      private static string ConvertURLToPlatformName(string url)
      {
        string host = new Uri(url).Host.ToLower();
        string[] parts = host.Split('.');
        if (parts.Length > 2 && parts[^1].Length == 2)
          host = parts[^3];
        else if (parts.Length >= 2)
          host = parts[^2];
        return host[0].ToString().ToUpper() + host[1..];
      }

      private static string NormalizeApplePlatformName(string platform) =>
        ConvertURLToPlatformName($"https://{string.Join(
          " ",
          platform.Split(" (")[..^1]
        )}");

      private class ChromiumFields
      {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Note { get; set; }
      }

      private class FirefoxFields
      {
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FormActionOrigin { get; set; }
      }

      private class SafariFields
      {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Notes { get; set; }
      }

      private class ChromiumMap : ClassMap<ChromiumFields>
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

      private class FirefoxMap : ClassMap<FirefoxFields>
      {
        public FirefoxMap()
        {
          Map(model => model.Url).Name("url");
          Map(model => model.Username).Name("username");
          Map(model => model.Password).Name("password");
          Map(model => model.FormActionOrigin).Name("formActionOrigin");
        }
      }

      private class SafariMap : ClassMap<SafariFields>
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

    public class ExportData
    {
      public static string ToBare(List<ReadWritableDatabaseEntry> entries) =>
        "name,url,username,password,note\n" +
        string.Join("", entries.Select(Mapper.ToCSVLine));

      public static string ToEncrypted(List<ReadWritableDatabaseEntry> entries) =>
        "bmFtZSx1cmwsdXNlcm5hbWUscGFzc3dvcmQsbm90ZQ==\n" +
        string.Join("\n", entries.Select(Mapper.ToEncryptedCSVLine));
    }
  }
}
