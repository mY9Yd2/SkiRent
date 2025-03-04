using FluentResults;

using SkiRent.Shared.Contracts.Bookings;

namespace SkiRent.Api.Services.Bookings;

public interface IBookingService
{
    public Task<Result<CreatedBookingResponse>> CreateAsync(int userId, CreateBookingRequest request);
    public Task<Result<GetBookingResponse>> GetAsync(int bookingId, int userId, Func<string, bool> isInRole);
    public Task<Result<IEnumerable<GetAllBookingResponse>>> GetAllAsync(int userId, Func<string, bool> isInRole);
    public Task<Result<GetBookingResponse>> UpdateAsync(int bookingId, UpdateBookingRequest request);
}
