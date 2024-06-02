using System.Text;
using System.Globalization;

namespace Passenger
{
  public static class Generator
  {
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
      {"5", new(){"S","s","$"}},
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

    private static readonly string specials = "@_.-€£$~!#%^&*()+=[]{}|;:,<>?/";
    private static readonly string lowers = "abcdefghijklmnopqrstuvyz";
    private static readonly string uppers = lowers.ToUpper(CultureInfo.InvariantCulture);
    private static readonly string numbers = "0123456789";
    private static readonly string chars = lowers + uppers + numbers + specials;

    private static readonly Random random = new();

    public static string Manipulated(string input)
    {
      StringBuilder passphrase = new();
      foreach (char character in input)
      {
        string lowerChar = character.ToString().ToLower(CultureInfo.InvariantCulture);
        passphrase.Append(manipulate.TryGetValue(
          lowerChar,
          out List<string> value
        ) ? value[random.Next(value.Count)]
          : lowerChar
        );
      }

      return passphrase.ToString();
    }

    public static string New(int wantedLength = 32)
    {
      int length = wantedLength < 8
        ? 8
        : wantedLength;

      StringBuilder passphrase = new();

      // Generate a passphrase with the specified length
      for (int i = passphrase.Length; i < length; i++)
        passphrase.Append(chars[random.Next(chars.Length)]);

      /**
       * ! We do not use a while loop to ensure that
       * the passphrase contains at least two characters 
       * from each set. Using a while loop, can result
       * in an infinite loop if the random number 
       * generator keeps generating the same index.
       */

      // Generate 8 different index
      List<int> positions = Enumerable.Range(0, length
      ).OrderBy(x => random.Next()
      ).Take(8
      ).ToList();

      // Ensure that the passphrase contains at least two characters from each set
      for (int position = 0; position < 8; ++position)
      {
        string set = (position % 4) switch
        {
          0 => lowers,
          1 => uppers,
          2 => numbers,
          _ => specials
        };
        passphrase[positions[position]] = set[random.Next(set.Length)];
      }

      return passphrase.ToString();
    }
  }
}