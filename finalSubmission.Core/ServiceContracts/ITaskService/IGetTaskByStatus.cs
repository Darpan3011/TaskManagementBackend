using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Enums;
namespace finalSubmission.Core.ServiceContracts.ITaskService
{
    public interface IGetTaskByStatus
    {
        public Task<List<MyTask>> GetTasksByAStatus(CustomTaskStatus customTaskStatus);
    }
}
