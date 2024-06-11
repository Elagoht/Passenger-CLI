using System.Globalization;

namespace Passenger.Tests
{
  public class GeneratorTests
  {
    [Fact]
    public void New_ReturnsPassphraseWithDefaultLength()
    {
      // Arrange

      // Act
      string passphrase = Generator.New();

      // Assert
      Assert.Equal(32, passphrase.Length);
    }

    [Theory]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(24)]
    [InlineData(64)]
    public void New_ReturnsPassphraseWithSpecifiedLength(int length)
    {
      // Arrange

      // Act
      string passphrase = Generator.New(length);

      // Assert
      Assert.Equal(length, passphrase.Length);
    }

    [Fact]
    public void New_ReturnsPassphraseWithAtLeastTwoCharactersFromEachSet()
    {
      // Arrange

      // Act
      string passphrase = Generator.New();

      string specials = "@_.-€£$~!#%^&*()+=[]{}|;:,<>?/";
      string lowers = "abcdefghijklmnopqrstuvyz";
      string uppers = lowers.ToUpper(CultureInfo.InvariantCulture);
      string numbers = "0123456789";

      // Assert
      Assert.Contains(passphrase, character => lowers.Contains(character));
      Assert.Contains(passphrase, character => uppers.Contains(character));
      Assert.Contains(passphrase, character => numbers.Contains(character));
      Assert.Contains(passphrase, character => specials.Contains(character));
    }
  }
}