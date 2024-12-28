using System.ComponentModel.DataAnnotations;

namespace finalSubmission.Core.Domain.Entities
{
    public class LoginUser
    {
        [Required(ErrorMessage = "UserName is required.")]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public required string Password { get; set; }
    }

}
