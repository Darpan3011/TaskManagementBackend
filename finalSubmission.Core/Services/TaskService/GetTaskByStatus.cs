using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.Enums;
using finalSubmission.Core.ServiceContracts.ITaskService;

namespace finalSubmission.Core.Services.TaskService
{
    public class GetTaskByStatus : IGetTaskByStatus
    {
        private readonly ITaskRepository _taskRepository;

        public GetTaskByStatus(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public async Task<List<MyTask>> GetTasksByAStatus(CustomTaskStatus customTaskStatus)
        {
            List<MyTask>? tasks = await _taskRepository.GetAllTasksByStatus(customTaskStatus);

            return tasks.ToList();
        }
    }
}
