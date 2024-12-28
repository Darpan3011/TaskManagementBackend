using finalSubmission.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace finalSubmission.Core.Domain.Entities
{
    public class MyTask
    {
        [Key]
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(100, ErrorMessage = "The {0} must be a maximum length of {1}.")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [MinLength(4, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(500, ErrorMessage = "The {0} must be a maximum length of {1}.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public required Guid UserId { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DataType(DataType.Date, ErrorMessage = "The {0} must be a valid date.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public required DateTime DueDate { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [EnumDataType(typeof(CustomTaskStatus), ErrorMessage = "The {0} must be a valid value.")]
        public CustomTaskStatus Status { get; set; } = CustomTaskStatus.Pending;
    }
}