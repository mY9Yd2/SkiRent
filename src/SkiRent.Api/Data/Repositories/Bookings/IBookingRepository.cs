using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.Bookings;

public interface IBookingRepository : IRepository<Booking>
{
    public Task<Booking?> GetBookingWithItemsAsync(Guid paymentId);
}
