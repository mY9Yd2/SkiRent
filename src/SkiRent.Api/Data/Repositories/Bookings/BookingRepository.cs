using Microsoft.EntityFrameworkCore;

using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.Bookings;

public class BookingRepository : BaseRepository<Booking, int>, IBookingRepository
{
    public BookingRepository(DbContext context) : base(context)
    { }

    public async Task<Booking?> GetBookingWithItemsAsync(Guid paymentId)
    {
        return await _dbSet
            .Include(booking => booking.BookingItems)
            .FirstOrDefaultAsync(booking => booking.PaymentId == paymentId);
    }

    public async Task<Booking?> GetBookingWithItemsAsync(int bookingId)
    {
        return await _dbSet
            .Include(booking => booking.BookingItems)
            .FirstOrDefaultAsync(booking => booking.Id == bookingId);
    }
}
