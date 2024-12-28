using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.Enums;
using finalSubmission.Core.ServiceContracts.ITaskService;

namespace finalSubmission.Core.Services.TaskService
{
    public class ChangeStatus : IChangeStatus
    {
        private readonly ITaskRepository _taskRepository;

        public ChangeStatus(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public async Task<MyTask> ChangeAStatus(string Title, CustomTaskStatus newStatus)
        {

            MyTask? myTask = await _taskRepository.UpdateTaskStatus(Title, newStatus);

            return myTask;
        }
    }
}
