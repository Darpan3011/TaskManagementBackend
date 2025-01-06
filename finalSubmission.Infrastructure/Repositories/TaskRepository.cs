using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.Domain.RepositoryContracts;
using finalSubmission.Core.DTO;
using finalSubmission.Core.Enums;
using finalSubmission.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace finalSubmission.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskOrderDbContext _context;

        public TaskRepository(TaskOrderDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all tasks from the database.
        /// </summary>
        /// <returns>Returns a list of all tasks.</returns>
        public async Task<List<MyTask>> GetAllTasks()
        {
            return await _context.AllTasksTable.ToListAsync();

        }

        /// <summary>
        /// Adds a new task to the database.
        /// </summary>
        /// <param name="myTask">The task to be added.</param>
        /// <returns>Returns the added task.</returns>
        public async Task<MyTask> AddNewTask(MyTask myTask)
        {
            await _context.AllTasksTable.AddAsync(myTask);
            await _context.SaveChangesAsync();
            return myTask;
        }

        /// <summary>
        /// Updates the status of an existing task.
        /// </summary>
        /// <param name="title">The title of the task to be updated.</param>
        /// <param name="newStatus">The new status to set for the task.</param>
        /// <returns>Returns the updated task if successful, or null if the task was not found.</returns>
        public async Task<MyTask?> UpdateTaskStatus(string title, CustomTaskStatus newStatus)
        {
            // Fetch the task using the title
            MyTask? myTask = await _context.AllTasksTable.FirstOrDefaultAsync(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));

            if (myTask != null)
            {
                myTask.Status = newStatus;
                _context.AllTasksTable.Update(myTask);
                await _context.SaveChangesAsync();
                return myTask;
            }

            return null;
        }

        /// <summary>
        /// Deletes a task from the database by its title.
        /// </summary>
        /// <param name="title">The title of the task to delete.</param>
        /// <returns>Returns true if the task was deleted successfully, otherwise false.</returns>
        public async Task<bool> DeleteExistingTask(string title)
        {
            MyTask? task = await GetTaskByTitle(title);

            if (task != null)
            {
                _context.AllTasksTable.Remove(task);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves tasks from the database that have a specific status.
        /// </summary>
        /// <param name="status">The status of tasks to retrieve.</param>
        /// <returns>Returns a list of tasks that match the given status.</returns>
        public async Task<List<MyTask>?> GetAllTasksByStatus(CustomTaskStatus status)
        {
            return await _context.AllTasksTable.Where(t => t.Status == status).ToListAsync();
        }

        /// <summary>
        /// Retrieves tasks from the database that are due before or on the specified date.
        /// </summary>
        /// <param name="dueDate">The due date to filter tasks by.</param>
        /// <returns>Returns a list of tasks that are due before or on the specified date.</returns>
        public async Task<List<MyTask>?> GetAllTasksByDueDate(DateTime dueDate)
        {
            return await _context.AllTasksTable.Where(t => t.DueDate.Date <= dueDate.Date).ToListAsync();
        }

        /// <summary>
        /// Retrieves a task by its title.
        /// </summary>
        /// <param name="Title">The title of the task to retrieve.</param>
        /// <returns>Returns the task with the given title, or null if no such task exists.</returns>
        public async Task<MyTask?> GetTaskByTitle(string Title)
        {
            if (string.IsNullOrWhiteSpace(Title)) return null;

            MyTask? myTask = await _context.AllTasksTable.FirstOrDefaultAsync(t => t.Title == Title);

            return myTask;
        }

        /// <summary>
        /// Updates an existing task's details (description, due date, status, and user).
        /// </summary>
        /// <param name="myTask">The task with updated details.</param>
        /// <returns>Returns the updated task.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the task with the provided title does not exist.</exception>
        public async Task<MyTask> EditATask(MyTask myTask)
        {
            MyTask? existingTask = await _context.AllTasksTable.FirstOrDefaultAsync(t => t.Title == myTask.Title);

            if (existingTask == null)
            {
                throw new KeyNotFoundException("Task with the provided title does not exist.");
            }

            existingTask.Description = myTask.Description;
            existingTask.DueDate = myTask.DueDate;
            existingTask.Status = myTask.Status;

            if (myTask?.UserId != null)
            {
                existingTask.UserId = myTask.UserId;
            }

            await _context.SaveChangesAsync();

            return existingTask;
        }

        /// <summary>
        /// Retrieves tasks assigned to a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user to get tasks for.</param>
        /// <returns>Returns a list of tasks assigned to the specified user.</returns>
        public async Task<List<MyTask>?> GetAllTasksByUserID(Guid userId)
        {
            return await _context.AllTasksTable.Where(temp => temp.UserId == userId).ToListAsync();
        }

        public async Task<List<MyTaskWithUsername>?> getTasksByDuedateAndStatus(
    Guid? userId,
    DateTime? dateTime,
    CustomTaskStatus? status,
    string? title)
        {
            // Start with the base query
            IQueryable<MyTask> query = _context.AllTasksTable.AsQueryable();

            // Apply filters
            if (userId.HasValue)
            {
                query = query.Where(task => task.UserId == userId.Value);
            }

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(task => task.Title.Contains(title));
            }

            if (dateTime.HasValue)
            {
                query = query.Where(task => task.DueDate <= dateTime.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(task => task.Status == status.Value);
            }

            // Join with UsersTable to get UserName
            List<MyTaskWithUsername>? result = await (from task in query
                join user in _context.AllUsersTable on task.UserId equals user.UserId
                select new MyTaskWithUsername
                {
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    UserName = user.UserName
                }).ToListAsync();

            return result;
        }

        public async Task<List<MyTaskWithUsername>?> GetAllTasksIncludingUsername()
        {
            return await (
                from task in _context.AllTasksTable
                join user in _context.AllUsersTable on task.UserId equals user.UserId
                select new MyTaskWithUsername
                {
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    UserName = user.UserName
                }).ToListAsync();
        }
    }
}
