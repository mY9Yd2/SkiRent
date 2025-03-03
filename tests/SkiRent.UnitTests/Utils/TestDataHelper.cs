using AutoFixture;

using SkiRent.Shared.Contracts.Common;

namespace SkiRent.UnitTests.Utils
{
    public static class TestDataHelper
    {
        public static PersonalDetails CreatePersonalDetails(Fixture fixture)
        {
            return fixture.Build<PersonalDetails>()
                .With(details => details.FullName, "John Doe")
                .With(details => details.PhoneNumber, "06301234567")
                .With(details => details.MobilePhoneNumber, "+36301234567")
                .With(details => details.Address, CreateAddress(fixture))
                .Create();
        }

        public static Address CreateAddress(Fixture fixture)
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
