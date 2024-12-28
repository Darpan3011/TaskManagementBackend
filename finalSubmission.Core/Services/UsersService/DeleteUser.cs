using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.ServiceContracts.IUserService;

namespace finalSubmission.Core.Services.UsersService
{
    public class DeleteUser : IDeleteUser
    {
        private readonly IUserRepository _userRepository;

        public DeleteUser(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User?> DeleteAnUser(string UserName)
        {
            // Attempt to delete the user via the repository
            return await _userRepository.DeleteUser(UserName);
        }

    }
}
