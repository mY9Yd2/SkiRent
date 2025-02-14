using SkiRent.Api.Data.Repositories.Equipments;
using SkiRent.Api.Data.Repositories.Users;

namespace SkiRent.Api.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly SkiRentContext _context;

    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<IEquipmentRepository> _equipmentRepository;

    public IUserRepository Users => _userRepository.Value;
    public IEquipmentRepository Equipments => _equipmentRepository.Value;

    public UnitOfWork(SkiRentContext context)
    {
        _context = context;

        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(_context));
        _equipmentRepository = new Lazy<IEquipmentRepository>(() => new EquipmentRepository(_context));
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
