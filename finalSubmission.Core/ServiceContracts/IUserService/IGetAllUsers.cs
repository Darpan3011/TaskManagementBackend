using finalSubmission.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finalSubmission.Core.ServiceContracts.IUserService
{
    public interface IGetAllUsers
    {
        public Task<List<User>> GetAllAUsers();
    }
}
