using SkiRent.Shared.Validators.Common.Users;

namespace SkiRent.UnitTests.Systems.Shared.Validators.Common.Users
{
    public class TestPasswordValidator
    {
        private PasswordValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new PasswordValidator();
        }

        [TestCase("Aa0!?*.@#$%^&+=", true)]
        [TestCase("fQUmZe36YRPZqf56", true)]
        [TestCase("bLZy6M2eahKCHvS3a", false)]
        [TestCase("Dx8G7pZ", false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("!?*.@#$%^&+=", false)]
        [TestCase("AAAAAAAAA", false)]
        [TestCase("aaaaaaaaa", false)]
        [TestCase("000000000", false)]
        public void ShouldReturnExpectedResult(string input, bool expectedIsValid)
        {
            // Act
            var result = _validator.Validate(input);

            // Assert
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
        }
    }
}
