using System.Security.Cryptography;
using System.Text;

namespace Passenger
{
  /**
   * This class written as a static class
   * to prevent thread safety issues.
   */
  public static class Crypto
  {
    private static readonly int NonceSize = 12;
    private static readonly int TagSize = 16;

    // Convert the secret key to a 256-bit key using SHA-256
    private static byte[] GetKey(string secretKey) =>
        SHA256.HashData(Encoding.UTF8.GetBytes(secretKey));

    public static string EncryptData(string secretKey, string data)
    {
      if (string.IsNullOrEmpty(secretKey))
        Error.SecretKeyNotProvided();

      byte[] key = GetKey(secretKey);
      byte[] plaintext = Encoding.UTF8.GetBytes(data);

      // Generate a nonce
      byte[] nonce = new byte[NonceSize];
      RandomNumberGenerator.Fill(nonce);

      byte[] ciphertext = new byte[plaintext.Length];
      byte[] tag = new byte[TagSize];

      // Encrypt the data
      using (AesGcm aesGcm = new(key, TagSize))
        aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);

      // Combine nonce, ciphertext, and tag into a single array
      byte[] encryptedData = new byte[NonceSize + ciphertext.Length + TagSize];
      Buffer.BlockCopy(nonce, 0, encryptedData, 0, NonceSize);
      Buffer.BlockCopy(ciphertext, 0, encryptedData, NonceSize, ciphertext.Length);
      Buffer.BlockCopy(tag, 0, encryptedData, NonceSize + ciphertext.Length, TagSize);

      // Return the result as a Base64 string
      return Convert.ToBase64String(encryptedData);
    }

    public static string DecryptData(string secretKey, string data)
    {
      if (string.IsNullOrEmpty(secretKey))
        Error.SecretKeyNotProvided();

      byte[] key = GetKey(secretKey);
      byte[] encryptedData = Convert.FromBase64String(data);

      // Extract nonce, tag, and ciphertext from the encrypted data
      byte[] nonce = new byte[NonceSize];
      byte[] tag = new byte[TagSize];
      byte[] ciphertext = new byte[encryptedData.Length - NonceSize - TagSize];

      Buffer.BlockCopy(encryptedData, 0, nonce, 0, NonceSize);
      Buffer.BlockCopy(encryptedData, encryptedData.Length - TagSize, tag, 0, TagSize);
      Buffer.BlockCopy(encryptedData, NonceSize, ciphertext, 0, ciphertext.Length);

      byte[] plaintext = new byte[ciphertext.Length];

      // Decrypt the data
      using (AesGcm aesGcm = new(key, TagSize))
        aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);

      // Return the decrypted plaintext as a UTF-8 string
      return Encoding.UTF8.GetString(plaintext);
    }
  }
}
