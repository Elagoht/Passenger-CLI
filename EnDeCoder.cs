namespace Passenger
{
  public static class EnDeCoder
  {
    // 256-bit, 32-character secret key
    public const string JSWSecret = "selamiabininselamivarmaalanyokki";

    public static string Encode(string data)
    {
      char[] charArray = data.ToCharArray();
      Array.Reverse(charArray);
      return new string(charArray);
    }

    public static string Decode(string data)
    {
      char[] charArray = data.ToCharArray();
      Array.Reverse(charArray);
      return new string(charArray);
    }
  }
}