using finalSubmission.Core.Domain.Entities;

namespace finalSubmission.Core.ServiceContracts.IUserService
{
    public interface IDeleteUser
    {
        public Task<User> DeleteAnUser(string UserName);
    }
}
