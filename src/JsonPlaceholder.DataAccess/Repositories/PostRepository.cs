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

        var sqlMaxId = "select top (1) id from jph_posts order by id desc";
        var lastId = await conn.ExecuteScalarAsync<int>(sqlMaxId);

        int newId = lastId + 1;
        post.Id = newId;

        var sqlInsert = @"insert into jph_posts (id, userId, title, body)
                values (@Id, @UserId, @Title, @Body);
                select * from jph_posts where id = @Id";
        var newPost = await conn.QueryFirstOrDefaultAsync<Post>(sqlInsert, post);

        return newPost;
    }

    public async Task<Post?> UpdateAsync(Post post)
    {
        using var conn = connectionFactory.CreateConnection();
        var sql = @"update jph_posts set userId = @UserId, title = @Title, body = @Body where id = @Id;
                    select * from jph_posts where id = @Id";
        var modifiedPost = await conn.QueryFirstOrDefaultAsync<Post>(sql, post);

        return modifiedPost;
    }

    public async Task<int> DeleteAsync(int id)
    {
        using var conn = connectionFactory.CreateConnection();
        var sql = "delete from jph_posts where id = @id";
        var numRows = await conn.ExecuteAsync(sql, new { id });

        return numRows;
    }
}
