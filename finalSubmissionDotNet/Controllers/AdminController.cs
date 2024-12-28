using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.Enums;
using finalSubmission.Core.ServiceContracts.ITaskService;
using finalSubmission.Core.ServiceContracts.IUserService;
using finalSubmissionDotNet.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace finalSubmissionDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AdminController : ControllerBase
    {
        private readonly IGetAllTasks _getAllTasks;
        private readonly ICreateTask _createTask;
        private readonly IGetTaskByTitle _getTaskByTitle;
        private readonly IDeleteTask _deleteTask;
        private readonly IEditTask _editTask;
        private readonly IGetTaskByDueDate _getTaskByDueDate;
        private readonly IGetTaskByStatus _getTaskByStatus;
        private readonly IGetAllUsers _getAllUsers;
        private readonly IGetByUserID _getByUserID;
        private readonly IUserExistsOrNot _userExistsOrNot;
        private readonly ITaskRepository _taskRepository;

        public AdminController(IGetAllTasks getAllTasks, ICreateTask createTask, IGetTaskByTitle getTaskByTitle, IDeleteTask deleteTask, IEditTask editTask, IGetTaskByDueDate getTaskByDueDate, IGetTaskByStatus getTaskByStatus, IGetAllUsers getAllUsers, IGetByUserID getByUserID, IUserExistsOrNot userExistsOrNot, ITaskRepository taskRepository)
        {
            _getAllTasks = getAllTasks;
            _createTask = createTask;
            _getTaskByTitle = getTaskByTitle;
            _deleteTask = deleteTask;
            _editTask = editTask;
            _getTaskByStatus = getTaskByStatus;
            _getTaskByDueDate = getTaskByDueDate;
            _getAllUsers = getAllUsers;
            _getByUserID = getByUserID;
            _userExistsOrNot = userExistsOrNot;
            _taskRepository = taskRepository;
        }

        /// <summary>
        /// Retrieves all tasks from the system.
        /// Returns 404 if no tasks are found.
        /// </summary>
        /// <returns>A list of tasks or a 404 if not found.</returns>
        [Authorize(Roles = "User, Admin")]
        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                List<MyTask>? myTasks;

                // Check if the user has the Admin role
                if (User.IsInRole("Admin"))
                {
                    // Admin role: Get all tasks
                    myTasks = await _getAllTasks.GetAllPossibleTasks();
                }
                else if (User.IsInRole("User"))
                {
                    // User role: Get tasks assigned to the current user
                    Guid userId = GetUserIdFromToken();

                    if (userId == Guid.Empty)
                    {
                        return Unauthorized(new { message = "User ID not found in token" });
                    }

                    myTasks = await _taskRepository.GetAllTasksByUserID(userId);
                }
                else
                {
                    return Forbid(); // This ensures the endpoint is accessed only by authorized roles
                }

                if (myTasks == null || myTasks.Count == 0)
                {
                    return NotFound(new { message = "No tasks found." });
                }

                return Ok(myTasks);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, new { message = "An error occurred while retrieving tasks.", details = ex.Message });
            }
        }


        /// <summary>
        /// Adds a new task to the system.
        /// Returns 400 if task creation fails.
        /// </summary>
        /// <param name="task">The task object to be added.</param>
        /// <returns>The created task or a 400 error if creation fails.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        [TypeFilter(typeof(ModelValidationActionFilter))]
        public async Task<IActionResult> AddTask([FromBody] MyTask task)
        {
            try
            {
                if (string.IsNullOrEmpty(task.Status.ToString()))
                {
                    task.Status = CustomTaskStatus.Pending;
                }

                bool isUserExists = await _userExistsOrNot.IsUserExistsOrNotExists("", task.UserId);
                if (isUserExists)
                {
                    MyTask? createdTask = await _createTask.CreateNewTask(task);
                    return Ok(createdTask);
                }
                else
                {
                    return BadRequest(new { message = $"User not found with UserID {task.UserId}" });
                }

            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException?.Message.Contains("Violation of PRIMARY KEY") == true)
            {
                return Conflict(new { message = "A task with this title already exists." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the task.", error = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a task based on its title.
        /// Returns 400 if the task cannot be deleted or 404 if not found.
        /// </summary>
        /// <param name="Title">The title of the task to be deleted.</param>
        /// <returns>Status of the delete operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("[action]/{Title}")]
        public async Task<IActionResult> DeleteTask(string Title)
        {
            try
            {
                if (await _getTaskByTitle.GetMyTaskByATitle(Title) is not null)
                {
                    bool isSuccess = await _deleteTask.DeleteATask(Title);
                    if (isSuccess)
                        return Ok();
                    else
                        return BadRequest(new { message = "Failed to delete the task." });
                }

                return NotFound(new { message = "Task not found." });
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Edits an existing task.
        /// Returns the updated task or 400 if editing fails.
        /// </summary>
        /// <param name="myTask">The task object with updated information.</param>
        /// <returns>The updated task or a 400 error if update fails.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("[action]")]
        [TypeFilter(typeof(ModelValidationActionFilter))]
        public async Task<IActionResult> EditTheTask(MyTask myTask)
        {
            try
            {
                await _editTask.EditATask(myTask);
                return Ok(myTask);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves tasks based on their due date.
        /// Returns 404 if no tasks are found for the given due date.
        /// </summary>
        /// <param name="dateTime">The due date to filter tasks by.</param>
        /// <returns>A list of tasks or a 404 error if no tasks are found.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]/{dateTime}")]
        public async Task<IActionResult> GetAllTaskByDueDate(DateTime dateTime)
        {
            try
            {
                List<MyTask> myTasks = await _getTaskByDueDate.GetTaskByADueDate(dateTime);

                if (myTasks is null || myTasks.Count == 0)
                {
                    return NotFound(new { message = $"No tasks found for the due date {dateTime.ToString()}." });
                }

                return Ok(myTasks);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves tasks based on their status.
        /// Returns 404 if no tasks are found for the given status.
        /// 0 for  Pending, 1 InProgress, 2 Completed
        /// </summary>
        /// <param name="status">The status to filter tasks by.</param>
        /// <returns>A list of tasks or a 404 error if no tasks are found.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]/{status}")]
        public async Task<IActionResult> GetAllTaskByStatus(CustomTaskStatus status)
        {
            try
            {
                List<MyTask> myTasks = await _getTaskByStatus.GetTasksByAStatus(status);

                if (myTasks is null || myTasks.Count == 0)
                {
                    return NotFound(new { message = $"No tasks found for status {status}." });
                }

                return Ok(myTasks);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves all users in the system.
        /// </summary>
        /// <returns>A list of users.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                return Ok(await _getAllUsers.GetAllAUsers());
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Retrieves tasks assigned to a specific user by their user ID.
        /// Returns 404 if no tasks are found for the user.
        /// </summary>
        /// <param name="userID">The ID of the user whose tasks are being retrieved.</param>
        /// <returns>A list of tasks or a 404 error if no tasks are found for the user.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetTasksByUserID(Guid userID)
        {
            try
            {
                List<MyTask> myTasks = await _getByUserID.GetTaskByUserID(userID);

                if (myTasks is null || myTasks.Count == 0)
                {
                    return NotFound(new { message = $"No tasks found for user with ID {userID}." });
                }

                return Ok(myTasks);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private Guid GetUserIdFromToken()
        {
            Claim? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
        }
    }
}