using Dapper;
using JsonPlaceholder.Core.Entities;
using JsonPlaceholder.Core.Interfaces;
using JsonPlaceholder.DataAccess.Database;

namespace JsonPlaceholder.DataAccess.Repositories;

public class UserRepository : AbstractRepository, IUserRepository
{
    public UserRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var conn = connectionFactory.CreateConnection();
        var sql = "select * from jph_users order by id";
        var users = await conn.QueryAsync<User>(sql);

        return users;
    }
}
