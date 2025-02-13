using SkiRent.Api.Data.Repositories.Users;

namespace SkiRent.Api.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly SkiRentContext _context;

    private readonly Lazy<IUserRepository> _userRepository;

    public IUserRepository Users => _userRepository.Value;

    public UnitOfWork(SkiRentContext context)
    {
        _context = context;

        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(_context));
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
