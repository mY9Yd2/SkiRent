using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data.Repositories.Users;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(SkiRentContext context) : base(context)
    { }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await FindAsync(user => user.Email == email);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        return await FindAllAsync(user => user.UserRole == role);
    }
}
