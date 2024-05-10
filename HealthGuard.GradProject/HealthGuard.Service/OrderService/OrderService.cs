using HealthGuard.Core.Entities.Order;
using HealthGuard.Core.Entities;
using HealthGuard.Core.Repository.contract;
using HealthGuard.Core;
using HealthGuard.Core.Services.contract;
using HealthGuard.Core.Specifications.OrderSpecification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Service.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository BasketRepo, IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _basketRepo = BasketRepo;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string BuyerEmail, string BasketId, int DeliveryMethodId, ShippingAddress ShippingAddress)
        {
            var Basket = await _basketRepo.GetBasketAsync(BasketId);
            var OrderItems = new List<OrderItem>();
            if (Basket?.Items.Count > 0)
            {
                foreach (var item in Basket.Items)
                {
                    var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
                    var ProductItemOrder = new ProductItemOrder(product.Id, product.Name, product.PictureUrl);
                    var OrderItem = new OrderItem(ProductItemOrder, item.Quanntity, (int)product.Price);
                    OrderItems.Add(OrderItem);

                }
            }
            var SubTotal = OrderItems.Sum(i => i.Price * i.Quantity);
            var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(DeliveryMethodId);
            var Spec = new OrderWithPaymentIntentSpec(Basket.PaymentIntentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetWithSpecAsync(Spec);
            if (ExOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
                await _paymentService.CrreateOrUpdatePaymentIntent(BasketId);
            }
            var Order = new Order(BuyerEmail, ShippingAddress, DeliveryMethod, OrderItems, SubTotal, Basket.PaymentIntentId);
            await _unitOfWork.Repository<Order>().Add(Order);
            var Result = await _unitOfWork.CompleteAsync();
            if (Result <= 0)
                return null;
            return Order;

        }
        public Task<Order> CreateOrderByIdForSpecUserAsync(string BuyerEmail, int OrderId)
        {
            var Spec = new OrderSpecifications(BuyerEmail, OrderId);
            var Order = _unitOfWork.Repository<Order>().GetWithSpecAsync(Spec);
            return Order;
        }

        public async Task<IReadOnlyList<Order>> CreateOrderForSpecUserAsync(string BuyerEmail)
        {
            var Spec = new OrderSpecifications(BuyerEmail);
            var Order = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(Spec);
            return Order;
        }
    }
}
