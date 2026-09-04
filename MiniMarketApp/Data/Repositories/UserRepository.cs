using MiniMarketApp.Models;

namespace MiniMarketApp.Data.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly DatabaseContext _databaseContext;

    public UserRepository(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public User? GetByUserName(string userName)
    {
        using var connection = _databaseContext.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT UserId, UserName, PasswordHash, Role, IsActive
FROM Users
WHERE UserName = $userName
LIMIT 1;";
        command.Parameters.AddWithValue("$userName", userName);

        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        return new User
        {
            UserId = reader.GetInt32(0),
            UserName = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            Role = (UserRole)reader.GetInt32(3),
            IsActive = reader.GetInt32(4) == 1,
        };
    }

    public int Add(User user)
    {
        using var connection = _databaseContext.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
INSERT INTO Users (UserName, PasswordHash, Role, IsActive)
VALUES ($userName, $passwordHash, $role, $isActive);
SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("$userName", user.UserName);
        command.Parameters.AddWithValue("$passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("$role", (int)user.Role);
        command.Parameters.AddWithValue("$isActive", user.IsActive ? 1 : 0);

        return Convert.ToInt32((long)command.ExecuteScalar()!);
    }

    public void SoftDelete(int userId)
    {
        using var connection = _databaseContext.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
UPDATE Users
SET IsActive = 0,
    UpdatedAt = datetime('now')
WHERE UserId = $userId;";
        command.Parameters.AddWithValue("$userId", userId);
        command.ExecuteNonQuery();
    }
}
