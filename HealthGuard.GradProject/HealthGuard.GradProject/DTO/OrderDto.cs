using System.ComponentModel.DataAnnotations;

namespace HealthGuard.GradProject.DTO
{
    public class OrderDto
    {
        public int DeliveryMethodId { get; set; }
        public AddressDto shipToAddress { get; set; }
    }
}
