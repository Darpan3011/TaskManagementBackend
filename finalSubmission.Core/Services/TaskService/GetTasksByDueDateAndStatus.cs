using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.Enums;
using finalSubmission.Core.ServiceContracts.ITaskService;

namespace finalSubmission.Core.Services.TaskService
{
    public class GetTasksByDueDateAndStatus : IGetTasksByDueDateAndStatus
    {
        private readonly ITaskRepository _taskRepository;

        public GetTasksByDueDateAndStatus(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public async Task<List<MyTask>?> GetMyTasksfromDueDateandStatus(Guid? userId, DateTime? dateTime, CustomTaskStatus? customTaskStatus, string? title)
        {
            List<MyTask>? myTasks = await _taskRepository.getTasksByDuedateAndStatus(userId, dateTime, customTaskStatus, title);

            return myTasks;
        }
    }
}
