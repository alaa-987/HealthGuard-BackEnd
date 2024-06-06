using HealthGuard.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace HealthGuard.GradProject.DTO
{
    public class CustomerWishlistDto
    {
        [Required]
        public string Id { get; set; }
        public List<WishlistItemDto> Items { get; set; }
    }
}
