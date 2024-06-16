using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Passenger
{
  /// <summary>Final model to use on database file</summary>
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

  /// <summary>Model to track passphrase changes over time</summary>
  public class TrackablePassphrase
  {
    [JsonPropertyName("passphrase"), Required]
    public string Passphrase { get; set; }
    [JsonPropertyName("updated"), Required]
    public string Updated { get; set; }
  }

  /// <summary>Model to use on JWT related operations</summary>
  public class Credentials
  {
    public string Username { get; set; }
    public string Passphrase { get; set; }
  }

  /// <summary>Key-value pair model for constants</summary>
  public class ConstantPair
  {
    [JsonPropertyName("key"), Required]
    public string Key { get; set; }
    [JsonPropertyName("value"), Required]
    public string Value { get; set; }
  }

  /// <summary>Minimum details to use on statistics</summary>
  public class MicroDatabaseEntry
  {
    [JsonPropertyName("platform"), Required]
    public string Platform { get; set; }
    [JsonPropertyName("id"), Required]
    public string Id { get; set; }
    [JsonPropertyName("url"), Required]
    public string Url { get; set; }
  }

  /// <summary>Details to use on access counts logs</summary>
  public class CountableDatabaseEntry : MicroDatabaseEntry
  {
    [JsonPropertyName("totalAccesses"), Required]
    public int TotalAccesses { get; set; }
  }

  /// <summary>Details to use on list of entries such as fetching all</summary>
  public class ListableDatabaseEntry : CountableDatabaseEntry
  {
    [JsonPropertyName("identity"), Required]
    public string Identity { get; set; }
    [JsonPropertyName("created"), Required] // Auto-generated
    public string Created { get; set; }
    [JsonPropertyName("updated"), Required] // Auto-generated
    public string Updated { get; set; }
  }

  /// <summary>Maximum details to use on CRUD operations</summary>
  public class DatabaseEntry : ListableDatabaseEntry
  {
    [JsonPropertyName("passphrase"), Required]
    public List<TrackablePassphrase> Passphrases { get; set; }
    [JsonPropertyName("notes")] // Optional
    public string Notes { get; set; }
  }
}