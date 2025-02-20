using SkiRent.Shared.Contracts.Common;
using SkiRent.Shared.Validators.Common.Users;

namespace SkiRent.UnitTests.Systems.Validators.Common.Users
{
    public class TestRoleValidator
    {
        private RoleValidator _validator;

        [SetUp]
        public void Setup()
        {
            _validator = new RoleValidator();
        }

        [TestCase(RoleTypes.Admin, true)]
        [TestCase(RoleTypes.Customer, true)]
        [TestCase(RoleTypes.Invalid, false)]
        public void ShouldReturnExpectedResult(RoleTypes input, bool expectedIsValid)
        {
            // Act
            var result = _validator.Validate(input);

            // Assert
            Assert.That(result.IsValid, Is.EqualTo(expectedIsValid));
        }
    }
}
