using SkiRent.Shared.Validators.Common.Users;

namespace SkiRent.UnitTests.Systems.Shared.Validators.Common.Users
{
    public class TestEmailValidator
    {
        private EmailValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new EmailValidator();
        }

        [TestCase("test@example.com", true)]
        [TestCase("testexample.com", false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        public void ShouldReturnExpectedResult(string input, bool expectedIsValid)
        {
            // Act
            var result = _validator.Validate(input);

            // Assert
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
        }

        [Test]
        public void ShouldFail_WhenInputExceedsMaxLength()
        {
            // Arrange
            var localPart = new string('a', 88 + 1);
            var input = $"{localPart}@example.com";

            // Act
            var result = _validator.Validate(input);

            // Assert
            Assert.That(result.IsValid, Is.False);
        }
    }
}
