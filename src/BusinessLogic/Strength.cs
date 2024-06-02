using System.Text.RegularExpressions;

namespace Passenger
{
  public partial class Strength
  {
    private enum StrengthCriteria
    {
      Lowercase,
      Uppercase,
      Numbers,
      Special,
      Repeated,
      SequentialNumbers,
      SequentialLetters,
      Short,
      Medium,
      Long,
      VeryLong,
      ExtremelyLong
    }

    private static readonly Dictionary<StrengthCriteria, Regex> Criterias = new()
    {
      [StrengthCriteria.Lowercase] = LowercaseRegex(),
      [StrengthCriteria.Uppercase] = UppercaseRegex(),
      [StrengthCriteria.Numbers] = NumbersRegex(),
      [StrengthCriteria.Special] = SpecialRegex(),
      [StrengthCriteria.Repeated] = RepeatedRegex(),
      [StrengthCriteria.SequentialNumbers] = SequentialNumbersRegex(),
      [StrengthCriteria.SequentialLetters] = SequentialLettersRegex(),
      [StrengthCriteria.Short] = ShortRegex(),
      [StrengthCriteria.Medium] = MediumRegex(),
      [StrengthCriteria.Long] = LongRegex(),
      [StrengthCriteria.VeryLong] = VeryLongRegex(),
      [StrengthCriteria.ExtremelyLong] = ExtremelyLongRegex()
    };

    private static readonly Dictionary<StrengthCriteria, int> CriteriaScores = new()
    {
      [StrengthCriteria.Lowercase] = 1,
      [StrengthCriteria.Uppercase] = 1,
      [StrengthCriteria.Numbers] = 1,
      [StrengthCriteria.Special] = 1,
      [StrengthCriteria.Repeated] = -2,
      [StrengthCriteria.SequentialNumbers] = -1,
      [StrengthCriteria.SequentialLetters] = -1,
      [StrengthCriteria.Short] = 1,
      [StrengthCriteria.Medium] = 1,
      [StrengthCriteria.Long] = 1,
      [StrengthCriteria.VeryLong] = 1,
      [StrengthCriteria.ExtremelyLong] = 1
    };

    private static readonly Dictionary<StrengthCriteria, string> CriteriaMessages = new()
    {
      [StrengthCriteria.Lowercase] = "At least one lowercase letter",
      [StrengthCriteria.Uppercase] = "At least one uppercase letter",
      [StrengthCriteria.Numbers] = "At least one number",
      [StrengthCriteria.Special] = "At least one special character",
      [StrengthCriteria.Repeated] = "No more than 2 repeated characters",
      [StrengthCriteria.SequentialNumbers] = "No sequential numbers",
      [StrengthCriteria.SequentialLetters] = "No sequential letters",
      [StrengthCriteria.Short] = "At least 8 characters",
      [StrengthCriteria.Medium] = "At least 12 characters",
      [StrengthCriteria.Long] = "At least 16 characters",
      [StrengthCriteria.VeryLong] = "At least 20 characters",
      [StrengthCriteria.ExtremelyLong] = "At least 24 characters"
    };

    private static readonly Dictionary<int, string> ScoreTable = new()
    {
      [-2] = "Immediately change this",
      [-1] = "Do not consider this",
      [0] = "Good start",
      [1] = "Unacceptable",
      [2] = "Extremely weak",
      [3] = "Easily guessable",
      [4] = "Should be more varied",
      [5] = "Acceptable",
      [6] = "Good",
      [7] = "Strong",
      [8] = "Perfect"
    };

    private static readonly Dictionary<int, string> Colors = new()
    {
      [-2] = "#FF0000",
      [-1] = "#FF3300",
      [0] = "#FF6600",
      [1] = "#FF9900",
      [2] = "#FFCC00",
      [3] = "#FFFF00",
      [4] = "#CCFF00",
      [5] = "#99FF00",
      [6] = "#66FF00",
      [7] = "#33FF00",
      [8] = "#00FF00"
    };

    public static string Color(int score) => Colors[score];

    public static int Calculate(string password)
    {
      int score = -1;

      foreach (var criteria in Criterias)
        if (criteria.Value.IsMatch(password))
          score += CriteriaScores[criteria.Key];

      return score;
    }

    public static string CalculatedMessage(int score) => ScoreTable[score];

    public static Dictionary<string, bool> Evaluate(string password)
    {
      var result = new Dictionary<string, bool>();

      foreach (var criteria in Criterias)
        result[CriteriaMessages[criteria.Key]] = criteria.Value.IsMatch(password);

      return result;
    }

    [GeneratedRegex(@"[a-z]")]
    private static partial Regex LowercaseRegex();
    [GeneratedRegex(@"[A-Z]")]
    private static partial Regex UppercaseRegex();
    [GeneratedRegex(@"[0-9]")]
    private static partial Regex NumbersRegex();
    [GeneratedRegex(@"[^a-zA-Z0-9]")]
    private static partial Regex SpecialRegex();
    [GeneratedRegex(@"(.)\1{2,}")]
    private static partial Regex RepeatedRegex();
    [GeneratedRegex(@"(012|123|234|345|456|567|678|789|987|876|765|654|543|432|321|210)")]
    private static partial Regex SequentialNumbersRegex();
    [GeneratedRegex(@"(abc|bcd|cde|def|efg|fgh|ghi|hij|ijk|jkl|klm|lmn|mno|nop|opq|pqr|qrs|rst|stu|tuv|uvw|vwx|wxy|xyz|zyx|yxw|xwv|wvu|vut|uts|tsr|srq|rqo|qon|onm|nml|mlk|lkj|kji|jih|ihg|hgf|gfe|fed|edc|dcb|cba|qwe|wer|ert|rty|tyu|yui|uio|iop|asd|sdf|dfg|fgh|ghj|hjk|jkl|zxc|xcv|cvb|vbn|bnm|mnb|nbv|bvc|vcx|cxz|lkj|kjh|jhg|hgf|gfd|fds|dsa|poi|oiu|iuy|uyt|ytr|tre|rew|ewq)")]
    private static partial Regex SequentialLettersRegex();
    [GeneratedRegex(@"^.{8,}$")]
    private static partial Regex ShortRegex();
    [GeneratedRegex(@"^.{12,}$")]
    private static partial Regex MediumRegex();
    [GeneratedRegex(@"^.{16,}$")]
    private static partial Regex LongRegex();
    [GeneratedRegex(@"^.{20,}$")]
    private static partial Regex VeryLongRegex();
    [GeneratedRegex(@"^.{24,}$")]
    private static partial Regex ExtremelyLongRegex();
  }
}