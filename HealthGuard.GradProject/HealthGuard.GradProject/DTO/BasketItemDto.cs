using System.ComponentModel.DataAnnotations;

namespace HealthGuard.GradProject.DTO
{
    public class BasketItemDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string PicUrl { get; set; }
        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Price Must Be Greater Than Zero")]
        public decimal Price { get; set; }
        public string? Category { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity Must Be At Least One")]
        public int Quanntity { get; set; }
    }
}
