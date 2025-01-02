using finalSubmission.Core.DTO;

namespace finalSubmission.Core.ServiceContracts.ITaskService
{
    public interface IGetTasksWithUserName
    {
        Task<List<MyTaskWithUsername>?> AllTaskWithUsernames();
    }
}
