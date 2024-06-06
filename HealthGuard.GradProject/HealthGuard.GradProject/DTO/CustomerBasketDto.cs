using System.ComponentModel.DataAnnotations;

namespace HealthGuard.GradProject.DTO
{
    public class CustomerBasketDto
    {
        [Required]
        public string Id { get; set; }
        public List<BasketItemDto> Items { get; set; }
        //public string? PaymentIntentId { get; set; }
        //public string? ClientSecret { get; set; }
        //public int DeliveryMethodId { get; set; }
        public decimal TotalPrice { get; set; }
        public int TotalQuantity { get; set; }
    }
}
