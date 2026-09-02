using Dapper;
using JsonPlaceholder.Core.Entities;
using JsonPlaceholder.Core.Interfaces;
using JsonPlaceholder.DataAccess.Database;

namespace JsonPlaceholder.DataAccess.Repositories;

public class PostRepository : AbstractRepository, IPostRepository
{
    public PostRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
    {
    }

    public async Task<IEnumerable<Post>> GetAllAsync()
    {
        using var conn = connectionFactory.CreateConnection();
        var sql = "select * from jph_posts order by id";
        var users = await conn.QueryAsync<Post>(sql);

        return users;
    }
}
