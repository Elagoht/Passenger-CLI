using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
namespace Passenger
{
  class Authorization(string secretKey)
  {
    public string GenerateToken(string username, string passphrase)
    {
      ValidatePassphrase(username, passphrase);

      SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
      SecurityTokenDescriptor tokenDescriptor = new()
      {
        IssuedAt = DateTime.UtcNow,
        Issuer = "passenger-cli",
        TokenType = "Bearer",
        Expires = DateTime.UtcNow.AddMinutes(10),
        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
      };

      JwtSecurityTokenHandler tokenHandler = new();
      SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
      TokenValidationParameters validationParameters = new()
      {
        ValidateIssuer = true,
        ValidIssuer = "passenger-cli",
        ValidateAudience = false,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
      };

      JwtSecurityTokenHandler tokenHandler = new();
      try
      {
        tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken); return true;
      }
      catch { return false; }
    }

    public static bool ValidatePassphrase(string username, string passphrase)
    {
      // TODO: This is a placeholder for a more complex validation algorithm
      if (!Database.IsRegistered())
      {
        Console.WriteLine("passenger: not registered yet");
        Environment.Exit(1);
      }
      string passphraseOnDB = Database.GetCredentials().Passphrase;
      string usernameOnDB = Database.GetCredentials().Username;

      if (passphraseOnDB != passphrase || usernameOnDB != username)
      {
        Console.WriteLine("passenger: passphrase could not be validated");
        Environment.Exit(1);
      }
      return true;
    }
  }
}