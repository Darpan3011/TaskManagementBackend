using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.ServiceContracts.IUserService;

namespace finalSubmission.Core.Services.UsersService
{
    public class CreateUser : ICreateUser
    {
        private readonly IUserRepository _userRepository;

        public CreateUser(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> CreateAnUser(User user)
        {
            return await _userRepository.CreateUser(user);
        }

    }
}
