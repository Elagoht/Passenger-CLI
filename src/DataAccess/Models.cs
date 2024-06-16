using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Passenger
{
  public class DatabaseModel
  {

    [JsonPropertyName("username")]
    public string Username { get; set; }
    [JsonPropertyName("passphrase")]
    public string Passphrase { get; set; }
    [JsonPropertyName("entries")]
    public List<DatabaseEntry> Entries { get; set; }
    [JsonPropertyName("constants")]
    public List<ConstantPair> Constants { get; set; }
  }

  public class ConstantPair
  {
    [JsonPropertyName("key"), Required]
    public string Key { get; set; }
    [JsonPropertyName("value"), Required]
    public string Value { get; set; }
  }

  public class MicroDatabaseEntry
  {
    [JsonPropertyName("platform"), Required]
    public string Platform { get; set; }
    [JsonPropertyName("id"), Required]
    public string Id { get; set; }
    [JsonPropertyName("url"), Required]
    public string Url { get; set; }
  }

  public class CountableDatabaseEntry : MicroDatabaseEntry
  {
    [JsonPropertyName("totalAccesses"), Required]
    public int TotalAccesses { get; set; }
  }

  public class ListableDatabaseEntry : CountableDatabaseEntry
  {
    [JsonPropertyName("identity"), Required]
    public string Identity { get; set; }
    [JsonPropertyName("created"), Required] // Auto-generated
    public string Created { get; set; }
    [JsonPropertyName("updated"), Required] // Auto-generated
    public string Updated { get; set; }
  }

  public class DatabaseEntry : ListableDatabaseEntry
  {
    [JsonPropertyName("passphrase"), Required]
    public string Passphrase { get; set; }
    [JsonPropertyName("notes")] // Optional
    public string Notes { get; set; }
  }

  public class Credentials
  {
    public string Username { get; set; }
    public string Passphrase { get; set; }
  }
}