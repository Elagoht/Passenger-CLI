using System.Text;

namespace Passenger
{
  /// <summary>
  /// Generate random passwords and manipulate existing ones.
  /// </summary>
  public static class Generator
  {
    /// <summary>
    /// Match characters to another set of characters that look similar.
    /// </summary>
    private static readonly Dictionary<string, List<string>> manipulate = new()
    {
      {"q", new(){"Q","q"}},
      {"w", new(){"W","m","M","w"}},
      {"e", new(){"E","€","£","e"}},
      {"r", new(){"R","r"}},
      {"t", new(){"T","7","t"}},
      {"y", new(){"Y","h","y"}},
      {"u", new(){"U","u","n"}},
      {"i", new(){"I","1","i"}},
      {"o", new(){"O","0","o"}},
      {"p", new(){"P","p"}},
      {"a", new(){"A","4","@","a"}},
      {"s", new(){"S","$","5","s"}},
      {"d", new(){"D","d"}},
      {"f", new(){"F"}},
      {"g", new(){"G","6","9","g"}},
      {"h", new(){"H","y","h"}},
      {"j", new(){"J","j"}},
      {"k", new(){"K","k"}},
      {"l", new(){"L","l"}},
      {"z", new(){"Z","2","z"}},
      {"x", new(){"X","x"}},
      {"c", new(){"C","c"}},
      {"v", new(){"V","v"}},
      {"b", new(){"B","3","8","b"}},
      {"n", new(){"N","n","u"}},
      {"m", new(){"M","W","w","m"}},
      {"0", new(){"O","o","0"}},
      {"1", new(){"i","1"}},
      {"2", new(){"Z","z","2"}},
      {"3", new(){"B","3"}},
      {"4", new(){"A","4"}},
      {"5", new(){"S","s","5"}},
      {"6", new(){"G","6"}},
      {"7", new(){"7","?","T"}},
      {"8", new(){"B","8"}},
      {"9", new(){"g","9"}},
      {"@", new(){"A","a"}},
      {"$", new(){"S","s","5"}},
      {"€", new(){"E","e"}},
      {"£", new(){"E","e"}},
      {"?", new(){"7"}}
    };

    private static readonly string specials = "@_.";
    private static readonly string lowers = "abcdefghijklmnopqrstuvyz";
    private static readonly string letters = lowers + lowers.ToUpper();
    private static readonly string numbers = "0123456789";
    /// <summary>
    /// Character sets for password generation.
    /// </summary>
    private static readonly string chars = letters + numbers + specials;

    private static readonly Random random = new();

    /// <summary>
    /// Manipulate a passphrase by replacing characters with similar looking ones.
    /// </summary>
    /// <param name="input">original passphrase</param>
    /// <returns>manipulated passphrase</returns>
    public static string Manipulated(string input)
    {
      StringBuilder passphrase = new();
      foreach (char character in input)
      {
        string lowerChar = character.ToString().ToLower();
        passphrase.Append(manipulate.TryGetValue(
          lowerChar,
          out List<string> value
        ) ? value[random.Next(value.Count)]
          : lowerChar
        );
      }

      return passphrase.ToString();
    }

    /// <summary>Generate a new passphrase.</summary>
    /// <param name="length">length of the passphrase</param>
    /// <returns>new passphrase</returns>
    public static string New(int length = 32)
    {
      StringBuilder passphrase = new();
      for (int i = 0; i < length; i++)
        passphrase.Append(chars[random.Next(chars.Length)]);

      foreach (char special in specials)
        while (!passphrase.ToString().Contains(special))
        {
          int randomIndex = random.Next(passphrase.Length);
          if (!specials.Contains(passphrase[randomIndex]))
            passphrase[randomIndex] = special;
        }

      return passphrase.ToString();
    }
  }
}