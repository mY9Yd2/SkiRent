using System.Text.Json.Serialization;

namespace SkiRent.Shared.Contracts.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Roles
    {
        Invalid,
        Admin,
        Customer
    }
}
