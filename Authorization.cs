using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
namespace Passenger
{
  /// <summary>
  /// Authorization methods
  /// </summary>
  /// <param name="secretKey"></param>
  /// <remarks>
  /// This class provides methods for generating and validating JWT tokens.
  /// </remarks>
  class Authorization(string secretKey)
  {
    private readonly string _secretKey = secretKey;

    /// <summary>
    /// Generate JWT token
    /// </summary>
    /// <param name="passphrase"></param>
    /// <returns>JWT token</returns>
    /// <remarks>
    /// This method generates a JWT token with the provided passphrase. This token is required to use other commands via command line.
    /// </remarks>
    public string GenerateToken(string passphrase)
    {
      ValidatePassphrase(passphrase);

      SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_secretKey));
      SecurityTokenDescriptor tokenDescriptor = new()
      {
        IssuedAt = DateTime.UtcNow,
        Issuer = "passenger-cli",
        TokenType = "Bearer",
        Expires = DateTime.UtcNow.AddMinutes(3),
        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
      };

      JwtSecurityTokenHandler tokenHandler = new();
      SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <param name="token"></param>
    /// <returns>True if token is valid, false otherwise</returns>
    /// <remarks>
    /// This method validates a JWT token.
    /// </remarks>
    public bool ValidateToken(string token)
    {
      TokenValidationParameters validationParameters = new()
      {
        ValidateIssuer = true,
        ValidIssuer = "passenger-cli",
        ValidateAudience = false,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey))
      };

      JwtSecurityTokenHandler tokenHandler = new();
      try
      {
        tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken); return true;
      }
      catch { return false; }
    }

    /// <summary>
    /// Validate passphrase
    /// </summary>
    /// <param name="passphrase"></param>
    /// <returns>True if passphrase is valid, false otherwise</returns>
    /// <remarks>
    /// This method validates a passphrase.
    /// </remarks>
    public static bool ValidatePassphrase(string passphrase)
    {
      // This is a placeholder for a more complex validation algorithm
      if (!Database.IsRegistered())
      {
        Console.WriteLine("passenger: not registered yet");
        Environment.Exit(1);
      }
      string result = Database.GetPassphrase();
      if (result != passphrase)
      {
        Console.WriteLine("passenger: passphrase could not be validated");
        Environment.Exit(1);
      }
      return true;
    }
  }
}