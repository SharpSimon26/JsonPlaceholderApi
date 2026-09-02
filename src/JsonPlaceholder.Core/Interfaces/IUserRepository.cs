using JsonPlaceholder.Core.Entities;

namespace JsonPlaceholder.Core.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
}