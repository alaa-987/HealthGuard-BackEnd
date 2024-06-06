namespace HealthGuard.GradProject.DTO
{
    public class BookingInfoDto
    {
        public string NurseName { get; set; }
        public decimal Price { get; set; }
        public string PatientName { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public int Age { get; set; }
    }
}
