using FluentResults;

using SkiRent.Shared.Contracts.Bookings;

namespace SkiRent.Api.Services.Bookings;

public interface IBookingService
{
    public Task<Result<CreatedBookingResponse>> CreateAsync(int userId, CreateBookingRequest request);
}
