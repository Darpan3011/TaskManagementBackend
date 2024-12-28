using finalSubmission.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finalSubmission.Core.ServiceContracts.ITaskService
{
    public interface IGetTaskByTitle
    {
        public Task<MyTask> GetMyTaskByATitle(string Title);
    }
}
