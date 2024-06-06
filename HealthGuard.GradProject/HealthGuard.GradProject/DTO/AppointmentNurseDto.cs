namespace HealthGuard.GradProject.DTO
{
    public class AppointmentNurseDto
    {
        public int AppointmentId { get; set; }
        public string NurseName { get; set; }
        public string PatientName { get; set; }
        public DateTime BookingDate { get; set; }
        public string StreetAddress { get; set; }
        public decimal NurseFees { get; set; }
    }
}
