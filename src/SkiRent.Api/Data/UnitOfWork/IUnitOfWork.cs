namespace SkiRent.Api.Data.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    public Task SaveChangesAsync();
}
