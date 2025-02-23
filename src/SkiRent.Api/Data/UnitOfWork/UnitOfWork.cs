using SkiRent.Api.Data.Repositories.Bookings;
using SkiRent.Api.Data.Repositories.EquipmentCategories;
using SkiRent.Api.Data.Repositories.Equipments;
using SkiRent.Api.Data.Repositories.Users;

namespace SkiRent.Api.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly SkiRentContext _context;

    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<IEquipmentRepository> _equipmentRepository;
    private readonly Lazy<IEquipmentCategoryRepository> _equipmentCategoryRepository;
    private readonly Lazy<IBookingRepository> _bookingRepository;

    public IUserRepository Users => _userRepository.Value;
    public IEquipmentRepository Equipments => _equipmentRepository.Value;
    public IEquipmentCategoryRepository EquipmentCategories => _equipmentCategoryRepository.Value;
    public IBookingRepository Bookings => _bookingRepository.Value;

    public UnitOfWork(SkiRentContext context)
    {
        _context = context;

        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(_context));
        _equipmentRepository = new Lazy<IEquipmentRepository>(() => new EquipmentRepository(_context));
        _equipmentCategoryRepository = new Lazy<IEquipmentCategoryRepository>(() => new EquipmentCategoryRepository(_context));
        _bookingRepository = new Lazy<IBookingRepository>(() => new BookingRepository(_context));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _context.Dispose();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
