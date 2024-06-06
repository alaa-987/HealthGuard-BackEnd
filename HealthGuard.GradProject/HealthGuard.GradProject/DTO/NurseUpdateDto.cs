namespace HealthGuard.GradProject.DTO
{
    public class NurseUpdateDto
    {
        public Guid Id { get; set; }
        public string NurseName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string PicUrl { get; set; }
        public string Hospital { get; set; }
        public string Specialty { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
