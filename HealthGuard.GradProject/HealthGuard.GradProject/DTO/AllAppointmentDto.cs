using System.ComponentModel.DataAnnotations;

namespace HealthGuard.GradProject.DTO
{
    public class AllAppointmentDto
    {
        public int Id { get; set; }
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
        public SpecificNusreDto AppNurse { get; set; }
        public SpecificUserDto User { get; set; }
    }
}
