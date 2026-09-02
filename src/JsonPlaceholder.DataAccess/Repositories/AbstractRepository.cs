using JsonPlaceholder.DataAccess.Database;

namespace JsonPlaceholder.DataAccess.Repositories;

public class AbstractRepository
{
    protected readonly IDbConnectionFactory connectionFactory;

    protected AbstractRepository(IDbConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }
}