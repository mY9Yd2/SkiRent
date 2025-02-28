using AutoFixture;

using SkiRent.Shared.Contracts.Auth;
using SkiRent.Shared.Contracts.Bookings;
using SkiRent.Shared.Contracts.Common;
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

        public static CreateBookingRequest CreateBooking(Fixture fixture, IEnumerable<EquipmentBooking> equipmentBookings)
        {
            return fixture.Build<CreateBookingRequest>()
                .With(request => request.PersonalDetails, CreatePersonalDetails(fixture))
                .With(request => request.Equipments, equipmentBookings)
                .With(request => request.StartDate, DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().UtcDateTime))
                .With(request => request.EndDate, DateOnly.FromDateTime(TimeProvider.System.GetUtcNow().AddDays(7).UtcDateTime))
                .With(request => request.SuccessUrl, new Uri("http://localhost/successful"))
                .With(request => request.CancelUrl, new Uri("http://localhost/failed"))
                .Create();
        }

        private static PersonalDetails CreatePersonalDetails(Fixture fixture)
        {
            return fixture.Build<PersonalDetails>()
                .With(details => details.FullName, "John Doe")
                .With(details => details.PhoneNumber, "06301234567")
                .With(details => details.MobilePhoneNumber, "+36301234567")
                .With(details => details.Address, CreateAddress(fixture))
                .Create();
        }

        private static Address CreateAddress(Fixture fixture)
        {
            return fixture.Build<Address>()
                .With(address => address.Country, "Magyarország")
                .With(address => address.PostalCode, "6800")
                .With(address => address.City, "Hódmezővásárhely")
                .With(address => address.StreetAddress, "Kossuth tér 1")
                .Create();
        }
    }
}
