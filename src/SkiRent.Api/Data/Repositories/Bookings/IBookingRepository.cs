using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.Bookings;

public interface IBookingRepository : IRepository<Booking, int>
{
    public Task<Booking?> GetBookingWithItemsAsync(Guid paymentId);
}
