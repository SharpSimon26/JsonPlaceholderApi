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
        var posts = await conn.QueryAsync<Post>(sql);

        return posts;
    }

    public async Task<IEnumerable<Post>> GetByUserIdAsync(int userId)
    {
        using var conn = connectionFactory.CreateConnection();
        var sql = "select * from jph_posts where userId = @userId order by id";
        var posts = await conn.QueryAsync<Post>(sql, new { userId });

        return posts;
    }    

    public async Task<Post?> GetByIdAsync(int id)
    {
        using var conn = connectionFactory.CreateConnection();
        var sql = "select * from jph_posts where id = @id";
        var post = await conn.QueryFirstOrDefaultAsync<Post>(sql, new { id });

        return post;
    }

    public async Task<Post?> CreateAsync(Post post)
    {
        using var conn = connectionFactory.CreateConnection();
        // TODO
        throw new NotImplementedException();
    }

    public async Task<Post?> UpdateAsync(Post post)
    {
        using var conn = connectionFactory.CreateConnection();
        // TODO
        throw new NotImplementedException();
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var conn = connectionFactory.CreateConnection();
        var sql = "delete from jph_posts where id = @id";
        var numRows = await conn.ExecuteAsync(sql, new { id });

        return numRows;

    }
}
