using HealthGuard.Core.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace HealthGuard.GradProject.DTO
{
    public class NurseDto
    {
        public Guid Id { get; set; }
        [Required]
        public string NurseName { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string PicUrl { get; set; }
        public List<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
        [Required]
        public string Hospital { get; set; }
        [Required]
        public string Specialty { get; set; }
        public string Email { get; set; }  
        public string PhoneNumber { get; set; }
    }
}
