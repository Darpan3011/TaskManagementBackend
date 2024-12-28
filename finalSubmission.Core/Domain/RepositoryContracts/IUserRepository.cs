using finalSubmission.Core.Domain.Entities;

namespace finalSubmission.Core.Domain.RepositoryContracts
{
    public interface IUserRepository
    {
        Task<User?> CreateUser(User user);
        Task<User?> DeleteUser(string UserName);
        Task<bool> UserExists(string? UserName, Guid? guid);
        Task<List<User>> GetAllAnUsers();
    }
}
