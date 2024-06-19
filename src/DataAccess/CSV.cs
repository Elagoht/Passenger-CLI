using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Passenger
{
  public class CSV
  {
    public static List<Type> ReadTyped<Type, TMap>(string csvContent)
      where TMap : ClassMap<Type>
    {
      try
      {
        using StringReader reader = new(csvContent);
        using CsvReader csvReader = new(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
          MissingFieldFound = null,
          HeaderValidated = null
        });

        csvReader.Context.RegisterClassMap<TMap>();
        return csvReader.GetRecords<Type>().ToList();
      }
      catch (ReaderException) { Error.CSVFormatMissmatch(); throw; }
      catch (Exception exception) { Error.FileExceptions(exception); throw; }
    }
  }
}