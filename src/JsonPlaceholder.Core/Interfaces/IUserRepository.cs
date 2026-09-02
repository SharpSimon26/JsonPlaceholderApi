using JsonPlaceholder.Core.Entities;

namespace JsonPlaceholder.Core.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> CreateAsync(User user);
    Task<User?> UpdateAsync(User user);
    Task<int> DeleteAsync(int id);
}