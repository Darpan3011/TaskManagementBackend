using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Enums;

namespace finalSubmission.Core.ServiceContracts.ITaskService
{
    public interface IGetFilteredTasksByUser
    {
        Task<List<MyTask>?> FilterTasksByAnUser(Guid userId, string? title, CustomTaskStatus? status, DateTime? dueDate);

    }
}
