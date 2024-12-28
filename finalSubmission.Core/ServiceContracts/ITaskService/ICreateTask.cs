using finalSubmission.Core.Domain.Entities;

namespace finalSubmission.Core.ServiceContracts.ITaskService
{
    public interface ICreateTask
    {
        public Task<MyTask> CreateNewTask(MyTask myTask);
    }
}
