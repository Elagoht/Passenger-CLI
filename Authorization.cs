using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

class Authorization(string secretKey)
{
  private readonly string _secretKey = secretKey;

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
  public static bool ValidatePassphrase(string passphrase)
  {
    // This is a placeholder for a more complex validation algorithm
    bool result = passphrase == "nobodycanbreakthis";
    if (!result)
    {
      Console.WriteLine("passenger: passphrase could not be validated");
      Environment.Exit(1);
    }
    return result;
  }
}
