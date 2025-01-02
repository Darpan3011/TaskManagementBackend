using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using finalSubmission.Core.Domain.Entities;
using finalSubmission.Core.DTO;
using finalSubmission.Core.Enums;

namespace finalSubmission.Core.ServiceContracts.ITaskService
{
    public interface IGetTasksByDueDateAndStatus
    {
        public Task<List<MyTaskWithUsername>?> GetMyTasksfromDueDateandStatus(Guid? userId, DateTime? dateTime, CustomTaskStatus? customTaskStatus, string? title);
    }
}
