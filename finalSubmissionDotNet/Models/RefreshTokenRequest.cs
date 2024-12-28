using System.ComponentModel.DataAnnotations;

namespace finalSubmissionDotNet.Models
{
    public class RefreshTokenRequest
    {
        [Required]
        public required string RefreshToken { get; set; }
    }

}
