using finalSubmission.Core.Domain.Entities;

namespace finalSubmission.Core.ServiceContracts.ITaskService
{
    public interface IGetTaskByDueDate
    {
        public Task<List<MyTask>> GetTaskByADueDate(DateTime dateTime);
    }
}
