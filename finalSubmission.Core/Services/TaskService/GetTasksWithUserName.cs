using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.DTO;
using finalSubmission.Core.ServiceContracts.ITaskService;

namespace finalSubmission.Core.Services.TaskService
{
    public class GetTasksWithUserName : IGetTasksWithUserName
    {
        private readonly ITaskRepository _taskRepository;
        public GetTasksWithUserName(ITaskRepository taskRepository) {
            _taskRepository = taskRepository;
        }
        public async Task<List<MyTaskWithUsername>?> AllTaskWithUsernames()
        {
            List<MyTaskWithUsername>? myTasks = await _taskRepository.GetAllTasksIncludingUsername();

            return myTasks;
        }
    }
}
