using System.Text.Json.Serialization;

namespace SkiRent.Shared.Contracts.Common
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum BookingStatusTypes
    {
        Invalid,
        Pending,
        Paid,
        Cancelled,
        Returned
    }
}
