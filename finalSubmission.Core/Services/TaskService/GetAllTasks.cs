using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.ServiceContracts.ITaskService;

namespace finalSubmission.Core.Services.TaskService
{
    public class GetAllTasks : IGetAllTasks
    {
        private readonly ITaskRepository _taskRepository;

        public GetAllTasks(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<List<MyTask>> GetAllPossibleTasks()
        {
            List<MyTask> tasks = await _taskRepository.GetAllTasks();

            return tasks.ToList();
        }
    }
}
