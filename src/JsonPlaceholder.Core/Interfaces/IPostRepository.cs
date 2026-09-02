using JsonPlaceholder.Core.Entities;

namespace JsonPlaceholder.Core.Interfaces;

public interface IPostRepository
{
    Task<IEnumerable<Post>> GetAllAsync();
    Task<IEnumerable<Post>> GetByUserIdAsync(int userId);    
    Task<Post?> GetByIdAsync(int id);
    Task<Post?> CreateAsync(Post post);
    Task<Post?> UpdateAsync(Post post);
    Task<int> DeleteAsync(int id);
}