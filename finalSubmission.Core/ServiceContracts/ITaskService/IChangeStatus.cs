using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Enums;

namespace finalSubmission.Core.ServiceContracts.ITaskService
{
    public interface IChangeStatus
    {
        public Task<MyTask> ChangeAStatus(string Title, CustomTaskStatus newStatus);
    }
}
