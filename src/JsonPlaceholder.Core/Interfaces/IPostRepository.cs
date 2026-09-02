using JsonPlaceholder.Core.Entities;

namespace JsonPlaceholder.Core.Interfaces;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetAllAsync();
}