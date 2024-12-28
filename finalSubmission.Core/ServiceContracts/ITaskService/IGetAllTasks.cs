using finalSubmission.Core.Domain.Entities;

namespace finalSubmission.Core.ServiceContracts.ITaskService
{
    public interface IGetAllTasks
    {
        public Task<List<MyTask>> GetAllPossibleTasks();
    }
}