using AutoFixture;

using SkiRent.Shared.Contracts.Auth;
using SkiRent.Shared.Contracts.EquipmentCategories;
using SkiRent.Shared.Contracts.Equipments;
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
            return fixture.Build<CreateEquipmentCategoryRequest>()
                .With(category => category.Name, Guid.NewGuid().ToString())
                .Create();
        }

        public static IEnumerable<CreateEquipmentCategoryRequest> CreateManyEquipmentCategory(Fixture fixture, int count = 2)
        {
            return [.. Enumerable.Range(0, count).Select(_ => CreateEquipmentCategory(fixture))];
        }

        public static CreateEquipmentRequest CreateEquipment(Fixture fixture, int categoryId = 1)
        {
            return fixture.Build<CreateEquipmentRequest>()
                .With(request => request.Name, Guid.NewGuid().ToString())
                .With(request => request.Description, Guid.NewGuid().ToString())
                .With(request => request.CategoryId, categoryId)
                .With(request => request.PricePerDay, fixture.Create<decimal>())
                .With(request => request.AvailableQuantity, fixture.Create<int>())
                .Create();
        }

        public static IEnumerable<CreateEquipmentRequest> CreateManyEquipment(Fixture fixture, int count = 2, int categoryId = 1)
        {
            return [.. Enumerable.Range(0, count).Select(_ => CreateEquipment(fixture))];
        }
    }
}
