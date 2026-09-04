using MiniMarketApp.Data.Repositories;
using MiniMarketApp.Helpers;
using MiniMarketApp.Models;

namespace MiniMarketApp.Services;

public sealed class AuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public User? Login(string userName, string password)
    {
        var user = _userRepository.GetByUserName(userName);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        return PasswordHasher.Verify(password, user.PasswordHash) ? user : null;
    }
}
