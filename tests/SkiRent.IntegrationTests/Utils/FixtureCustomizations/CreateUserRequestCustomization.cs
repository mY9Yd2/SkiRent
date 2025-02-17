using AutoFixture;

using SkiRent.Shared.Contracts.Users;

namespace SkiRent.IntegrationTests.Utils.FixtureCustomizations
{
    public class CreateUserRequestCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<CreateUserRequest>(composer => composer
                .With(request => request.Email, "teszt@example.com")
                .With(request => request.Password, "Teszt1234"));
        }
    }
}
