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
  }

  /// <summary>Model to track passphrase statistics changes over time</summary>
  public class TrackablePassphrase
  {
    [JsonPropertyName("strength"), Required]
    public int Strength { get; set; }
    [JsonPropertyName("length"), Required]
    public int Length { get; set; }
    [JsonPropertyName("created"), Required]
    public string Created { get; set; }
  }

  /// <summary>Model to use on JWT related operations</summary>
  public class Credentials
  {
    public string Username { get; set; }
    public string Passphrase { get; set; }
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

  /// <summary>Details to use on read and write operations</summary>
  public class ReadWritableDatabaseEntry : ListableDatabaseEntry
  {
    [JsonPropertyName("passphrase"), Required]
    public string Passphrase { get; set; }
    [JsonPropertyName("notes")] // Optional
    public string Notes { get; set; }
  }

  /// <summary>Maximum detailed model to save on database file</summary>
  public class DatabaseEntry : ReadWritableDatabaseEntry
  {
    [JsonPropertyName("passphraseHistory")]
    public List<TrackablePassphrase> PassphraseHistory { get; set; }
  }
}