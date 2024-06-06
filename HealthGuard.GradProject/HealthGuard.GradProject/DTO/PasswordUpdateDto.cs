using System.ComponentModel.DataAnnotations;

namespace HealthGuard.GradProject.DTO
{
    public class PasswordUpdateDto
    {
        [Required]
        public string NewPassword { get; set; }
    }
}
