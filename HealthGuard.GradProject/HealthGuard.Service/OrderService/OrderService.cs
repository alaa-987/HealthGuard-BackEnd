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
using Microsoft.Extensions.Logging;
using HealthGuard.Service.PaymentService;
using Org.BouncyCastle.Bcpg;

namespace HealthGuard.Service.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IBasketRepository BasketRepo, IUnitOfWork unitOfWork)
        {
            _basketRepo = BasketRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, ShippingAddress shippingAddress)
        {
            var Basket = await _basketRepo.GetBasketAsync(basketId);
            if (Basket == null || Basket.Items == null || !Basket.Items.Any())
            {
                return null;
            }
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
            var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(deliveryMethodId);

            var Order = new Order(buyerEmail, shippingAddress, DeliveryMethod, OrderItems, SubTotal);
            await _unitOfWork.Repository<Order>().Add(Order);
            var Result = await _unitOfWork.CompleteAsync();

            if (Result <= 0)
            {
                return null;
            }

            foreach (var item in Basket.Items)
            {
                await _basketRepo.RemoveBasketItemAsync(basketId, item.Id);
            }
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

        public async Task DeleteOrderAsync(int orderId)
        {
            var order = await _unitOfWork.Repository<Order>().GetAsync(orderId);
            if (order != null)
            {
                _unitOfWork.Repository<Order>().Delete(order);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IReadOnlyList<Order>> GetAllOrdersAsync()
        {
            return await _unitOfWork.Repository<Order>()
                .GetAllWithSpecAsync(new OrdersWithItemsAndDeliverySpecification());
        }


        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(orderId);
            return await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);
        }

    }
}
