using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.ServiceContracts.IUserService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finalSubmission.Core.Services.UsersService
{
    public class GetAllUsers : IGetAllUsers
    {
        private readonly IUserRepository _userRepository;

        public GetAllUsers(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<List<User>> GetAllAUsers()
        {
            return _userRepository.GetAllAnUsers();
        }
    }
}
