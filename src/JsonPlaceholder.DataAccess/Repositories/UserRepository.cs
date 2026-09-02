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

    public async Task<User?> GetByIdAsync(int id)
    {
        using var conn = connectionFactory.CreateConnection();
        var sql = "select * from jph_users where id = @id";
        var user = await conn.QueryFirstOrDefaultAsync<User>(sql, new { id });

        return user;
    }

    public async Task<User?> CreateAsync(User user)
    {
        using var conn = connectionFactory.CreateConnection();

        var sqlMaxId = "select top (1) id from jph_users order by id desc";
        var lastId = await conn.ExecuteScalarAsync<int>(sqlMaxId);

        int newId = lastId + 1;
        user.Id = newId;
        var sqlInsert = @"insert into jph_users (id, name, username, email, phone, website) 
                        values (@Id, @Name, @Username, @Email, @Phone, @Website);
                        select * from jph_users where id = @Id";
        var newUser = await conn.QueryFirstOrDefaultAsync<User>(sqlInsert, user);

        return newUser;
    }

    public async Task<User?> UpdateAsync(User user)
    {
        using var conn = connectionFactory.CreateConnection();
        var sql = @"update jph_users set name = @Name, username = @Username, email = @Email,
                    phone = @Phone, website = @Website where id = @Id;
                    select * from jph_users where id = @Id";
        var modifiedUser = await conn.QueryFirstOrDefaultAsync<User>(sql, user);

        return modifiedUser;
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var conn = connectionFactory.CreateConnection();
        var sql = "delete from jph_users where id = @id";
        var numRows = await conn.ExecuteAsync(sql, new { id });

        return numRows;
    }
}
