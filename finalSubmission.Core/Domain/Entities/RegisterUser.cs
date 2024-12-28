using System.ComponentModel.DataAnnotations;

namespace finalSubmission.Core.Domain.Entities
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "UserName is required.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; set; }
    }

}
