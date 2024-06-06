using System.ComponentModel.DataAnnotations;

namespace HealthGuard.GradProject.DTO
{
    public class AddressDto
    {
        [Required]
        public string FName { get; set; }
        [Required]
        public string LName { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
      
    }
}
