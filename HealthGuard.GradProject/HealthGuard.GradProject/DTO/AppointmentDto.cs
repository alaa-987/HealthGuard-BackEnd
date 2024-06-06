using HealthGuard.Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace HealthGuard.GradProject.DTO
{
    public class AppointmentDto
    {
       
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public string PatientName { get; set; }

        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }

    }
}
