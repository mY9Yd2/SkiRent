using SkiRent.Desktop.Models;

namespace SkiRent.Desktop.Services;

public interface IUserService
{
    public CurrentUser? CurrentUser { get; set; }
}

public class UserService : IUserService
{
    public CurrentUser? CurrentUser { get; set; }
}
