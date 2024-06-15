namespace Passenger.Tests
{
  public class ValidateTests
  {
    [Fact]
    public void IfIsOnRepository_PasswordOnRepository_ErrorFoundOnRepository()
    {
      Random random = new();

      // Arrange
      for (int length = 8; length < 285; ++length)
      {
        // Load the password list
        List<string> passwordList = PasswordRepository.Load(length);
        // Select random passwords from the list
        string[] passwords = passwordList
          .OrderBy(_ => random.Next())
          .Take(10)
          .ToArray();

        foreach (string password in passwords)
        {
          // Act
          if (PasswordRepository.BinarySearch(
              PasswordRepository.Load(password.Length),
              password
            ) >= 0
          // Assert
          ) Assert.True(true, "Password found on repository as expected");
          else Assert.True(false, "Password not found on repository, but it should be");
        }
      }
    }
  }
}