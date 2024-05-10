using HealthGuard.Core.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthGuard.GradProject.DTO
{
    public class ProductToReturnDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? About { get; set; }
        public string PictureUrl { get; set; }
        public Decimal Price { get; set; }
        public Decimal Rate { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}
