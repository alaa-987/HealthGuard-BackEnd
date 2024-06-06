using HealthGuard.Core;
using HealthGuard.Core.Entities;
using HealthGuard.Core.Entities.Order;
using HealthGuard.Core.Repository.contract;
using HealthGuard.Core.Services.contract;
using HealthGuard.Core.Specifications.OrderSpecification;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthGuard.Service.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration, IBasketRepository basketRepository, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeKeys:Secretkey"];

            var basket = await _basketRepository.GetBasketAsync(basketId);
            if (basket == null)
            {
                return null;
            }

            var shippingPrice = 0m;
            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod?.Cost ?? 0m;
            }

            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Core.Entities.Product>().GetAsync(item.Id);
                if (product != null)
                {
                    item.Price = (int)product.Price;
                }
            }

            var service = new PaymentIntentService();
            PaymentIntent intent;

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)basket.Items.Sum(i => i.Quanntity * i.Price) + (long)shippingPrice,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)basket.Items.Sum(i => i.Quanntity * i.Price) + (long)shippingPrice
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            await _basketRepository.UpdateBasketAsync(basket);

            return basket;
        }

        public async Task<Order> UpdatePaymentIntentTosuccedOrFailed(string PaymentIntentId, bool isSuccess)
        {
            var spec = new OrderWithPaymentIntentSpec(PaymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetWithSpecAsync(spec);

            if (order == null)
            {
                throw new Exception($"Order with PaymentIntentId {PaymentIntentId} not found.");
            }

            order.Status = isSuccess ? OrderStatus.PaymentSuccessded : OrderStatus.PaymentFailed;

            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.CompleteAsync();

            return order;
        }

    }
}
