using SkiRent.Desktop.Utils;

namespace SkiRent.UnitTests.Systems.Desktop.Utils;

public class TestPasswordGeneratorHelper
{
    [TestCase(8, 16)]
    [TestCase(12, 20)]
    [TestCase(10, 10)]
    [Repeat(64)]
    public void LengthWithinBounds(int minLength, int maxLength)
    {
        // Act
        string password = PasswordGeneratorHelper.GeneratePassword(minLength, maxLength);

        // Assert
        Assert.That(password, Has.Length.InRange(minLength, maxLength));
    }

    [Test]
    [Repeat(32)]
    public void ContainsUpperLowerDigit()
    {
        // Act
        string password = PasswordGeneratorHelper.GeneratePassword();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(password.Any(char.IsUpper), Is.True, "Password should contain at least one uppercase letter");
            Assert.That(password.Any(char.IsLower), Is.True, "Password should contain at least one lowercase letter");
            Assert.That(password.Any(char.IsDigit), Is.True, "Password should contain at least one digit");
        });
    }

    [Test]
    public void IsRandomized()
    {
        // Act
        var password1 = PasswordGeneratorHelper.GeneratePassword();
        var password2 = PasswordGeneratorHelper.GeneratePassword();

        // Assert
        Assert.That(password1, Is.Not.EqualTo(password2), "Passwords should be different between calls");
    }

    [Test]
    public void DefaultLength_IsBetween8And16()
    {
        // Act
        var password = PasswordGeneratorHelper.GeneratePassword();

        // Assert
        Assert.That(password, Has.Length.InRange(8, 16));
    }
}
