using MiniMarketApp.Models;

namespace MiniMarketApp.Data.Repositories;

public interface IUserRepository
{
    User? GetByUserName(string userName);
    int Add(User user);
    void SoftDelete(int userId);
}
