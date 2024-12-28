using System.ComponentModel.DataAnnotations;

namespace finalSubmission.Core.Domain.Entities
{
    public class User
    {
        
        [Required(ErrorMessage = "{0} is required")]
        public Guid UserId { get; set; }
        
        [Key]
        [Required(ErrorMessage = "{0} is required")]
        public required string UserName { get; set; }

    }
}
