using System.Text;

namespace Passenger
{
  public class Generate
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

    private static readonly Random random = new();

    public static string Manipulated(string input)
    {
      StringBuilder passphrase = new();
      foreach (char character in input)
      {
        string lowerChar = character.ToString().ToLower();
        passphrase.Append(manipulate.ContainsKey(lowerChar)
          ? manipulate[lowerChar][random.Next(manipulate[lowerChar].Count)]
          : lowerChar);
      }

      return passphrase.ToString();
    }
  }
}