using AutoFixture;

using SkiRent.IntegrationTests.Utils.FixtureCustomizations;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.IntegrationTests.Utils
{
    public static class TestDataCustomizationHelper
    {
        public static CreateUserRequest CreateUser(Fixture fixture)
        {
            fixture.Customize(new CreateUserRequestCustomization());

            var createUserRequest = fixture.Create<CreateUserRequest>();

            fixture.Customize(new SignInRequestCustomization(createUserRequest));

            return createUserRequest;
        }
    }
}
