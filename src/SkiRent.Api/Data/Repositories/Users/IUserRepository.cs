using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.Users;

public interface IUserRepository : IRepository<User, int>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
}
