using AutoFixture;

using SkiRent.Shared.Contracts.Auth;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.IntegrationTests.Utils.FixtureCustomizations
{
    public class SignInRequestCustomization : ICustomization
    {
        private readonly CreateUserRequest _createUserRequest;

        public SignInRequestCustomization(CreateUserRequest createUserRequest)
        {
            _createUserRequest = createUserRequest;
        }

        public void Customize(IFixture fixture)
        {
            fixture.Customize<SignInRequest>(composer => composer
                .With(request => request.Email, _createUserRequest.Email)
                .With(request => request.Password, _createUserRequest.Password));
        }
    }
}
