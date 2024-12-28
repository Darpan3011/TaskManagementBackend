using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.Enums;
using finalSubmission.Core.ServiceContracts.ITaskService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace finalSubmissionDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class UserController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IGetFilteredTasksByUser _getFilteredTasksByUser;

        public UserController(ITaskRepository taskRepository, IGetFilteredTasksByUser getFilteredTasksByUser)
        {
            _taskRepository = taskRepository;
            _getFilteredTasksByUser = getFilteredTasksByUser;
        }

        /// <summary>
        /// Gives all the Tasks assigned to the logged in User
        /// </summary>
        /// <returns>Return a List of all Tasks assigned 404 if no tasks found</returns>
        [HttpGet("tasks")]
        public async Task<IActionResult> GetTasksAssignedToUser()
        {
            try
            {
                Guid userId = GetUserIdFromToken();

                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                List<MyTask>? tasks = await _taskRepository.GetAllTasksByUserID(userId);

                if (tasks == null || tasks.Count == 0)
                {
                    return NotFound(new { message = "No tasks found for the user." });
                }

                return Ok(tasks);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves the particular Task based on title
        /// Allows to change the status of if that task is assigned to the logged in user.
        /// </summary>
        /// <param name="taskTitle">Task of the Title for changing the Title</param>
        /// <param name="newStatus">New Status</param>
        /// <returns></returns>
        [HttpPut("tasks/{taskTitle}/status")]
        public async Task<IActionResult> UpdateTaskStatus(string taskTitle, CustomTaskStatus newStatus)
        {
            try
            {
                Guid userId = GetUserIdFromToken();

                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                MyTask? task = await _taskRepository.GetTaskByTitle(taskTitle);
                if (task == null)
                {
                    return NotFound(new { message = "Task not found." });
                }

                if (task.UserId != userId)
                {
                    return Unauthorized(new { message = "You are not authorized to update this task." });
                }

                task.Status = newStatus;  // Assuming Status is a string in MyTask

                MyTask updatedTask = await _taskRepository.EditATask(task);

                return Ok(updatedTask);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Filters tasks assigned to the logged-in user by title, status, and/or due date.
        /// </summary>
        /// <param name="title">Optional filter by task title (partial or full match).</param>
        /// <param name="status">Optional filter by task status.</param>
        /// <param name="dueDate">Optional filter by tasks due on or before this date.</param>
        /// <returns>Returns the filtered list of tasks.</returns>
        [HttpGet("tasks/filter")]
        public async Task<IActionResult> GetFilteredTasks([FromQuery] string? title, [FromQuery] CustomTaskStatus? status, [FromQuery] DateTime? dueDate)
        {
            if (string.IsNullOrEmpty(title) && status == null && dueDate == null)
            {
                return BadRequest("At least one filter must be provided.");
            }
            try
            {
                Guid userId = GetUserIdFromToken();

                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { message = "User ID not found in token" });
                }

                List<MyTask>? tasks = await _getFilteredTasksByUser.FilterTasksByAnUser(userId, title, status, dueDate);
                
                if (tasks == null || tasks.Count == 0)
                {
                    return NotFound(new { message = "No tasks found for the user." });
                }
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while filtering tasks.", details = ex.Message });
            }
        }


        /// <summary>
        /// Helper Function to get the UsedId Guid from the Token
        /// </summary>
        /// <returns>Returns Guid if present else returns null</returns>
        private Guid GetUserIdFromToken()
        {
            Claim? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }
    }
}
