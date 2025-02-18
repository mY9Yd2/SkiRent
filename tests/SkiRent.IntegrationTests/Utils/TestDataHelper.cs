using AutoFixture;

using SkiRent.Shared.Contracts.Auth;
using SkiRent.Shared.Contracts.EquipmentCategories;
using SkiRent.Shared.Contracts.Users;

namespace SkiRent.IntegrationTests.Utils
{
    public static class TestDataHelper
    {
        public static (CreateUserRequest CreateUserRequest, SignInRequest SignInRequest) CreateUser(Fixture fixture)
        {
            var createUserRequest = fixture.Build<CreateUserRequest>()
                .With(request => request.Email, $"{Guid.NewGuid()}@example.com")
                .With(request => request.Password, "Test1234")
                .Create();

            var signInRequest = fixture.Build<SignInRequest>()
                .With(request => request.Email, createUserRequest.Email)
                .With(request => request.Password, createUserRequest.Password)
                .Create();

            return (createUserRequest, signInRequest);
        }

        public static CreateEquipmentCategoryRequest CreateEquipmentCategory(Fixture fixture)
        {
            var createCategoryRequest = fixture.Build<CreateEquipmentCategoryRequest>()
                .With(request => request.Name, Guid.NewGuid().ToString())
                .Create();

            return createCategoryRequest;
        }
    }
}
