using finalSubmission.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace finalSubmission.Core.DTO
{
    public class MyTaskWithUsername
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public CustomTaskStatus Status { get; set; }
        public string UserName { get; set; }
    }
}
