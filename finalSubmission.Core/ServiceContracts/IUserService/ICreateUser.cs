using finalSubmission.Core.Domain.Entities;

namespace finalSubmission.Core.ServiceContracts.IUserService
{
    public interface ICreateUser
    {
        public Task<User> CreateAnUser(User user);
    }
}
