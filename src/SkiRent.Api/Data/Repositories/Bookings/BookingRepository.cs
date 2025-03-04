using Microsoft.EntityFrameworkCore;

using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.Bookings;

public class BookingRepository : BaseRepository<Booking, int>, IBookingRepository
{
    public BookingRepository(DbContext context) : base(context)
    { }

    public async Task<Booking?> GetBookingWithItemsAsync(Guid paymentId)
    {
        return await _dbSet.Where(booking => booking.PaymentId == paymentId)
            .Include(booking => booking.BookingItems)
            .FirstOrDefaultAsync();
    }

    public async Task<Booking?> GetBookingWithItemsAsync(int bookingId)
    {
        return await _dbSet.Where(booking => booking.Id == bookingId)
            .Include(booking => booking.BookingItems)
            .FirstOrDefaultAsync();
    }
}
