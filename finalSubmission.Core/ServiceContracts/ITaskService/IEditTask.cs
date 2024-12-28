using finalSubmission.Core.Domain.Entities;

namespace finalSubmission.Core.ServiceContracts.ITaskService
{
    public interface IEditTask
    {
        public Task<MyTask> EditATask(MyTask myTask);
    }
}
