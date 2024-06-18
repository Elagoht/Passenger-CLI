using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Passenger
{
  public static class FileSystem
  {
    private static readonly string secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");

    public static string Read(string fileName)
    {
      try
      {
        if (!File.Exists(fileName)) return "";
        return EnDeCoder.Decode(
          Crypto.DecryptData(
            secretKey,
            new StreamReader(fileName).ReadToEnd()
          )
        );
      }
      catch (Exception exception) { Error.FileExceptions(exception); throw; }
    }

    public static void Write(string fileName, string data)
    {
      try
      {
        File.WriteAllText(
          fileName,
          Crypto.EncryptData(
            secretKey,
            EnDeCoder.Encode(data)
          )
        );
      }
      catch (Exception exception) { Error.FileExceptions(exception); }
    }

    public static List<Type> ReadCSV<Type, TMap>(string fileName)
      where TMap : ClassMap<Type>
    {
      try
      {
        using StreamReader reader = new(fileName);
        using CsvReader csvReader = new(
          reader,
          new CsvConfiguration(CultureInfo.InvariantCulture)
          {
            MissingFieldFound = null,
            HeaderValidated = null
          }
        );

        csvReader.Context.RegisterClassMap<TMap>();
        return csvReader.GetRecords<Type>().ToList();
      }
      catch (Exception exception) { Error.FileExceptions(exception); throw; }
    }
  }
}