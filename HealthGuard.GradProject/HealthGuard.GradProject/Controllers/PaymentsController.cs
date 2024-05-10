using AutoMapper;
using HealthGuard.Core.Entities;
using HealthGuard.Core.Services.contract;
using HealthGuard.GradProject.DTO;
using HealthGuard.GradProject.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace HealthGuard.GradProject.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        const string endpointSecret = "whsec_d022c6eeca792458d7fc8774c162a0682d7e7102edc631a809fc9ce659e318c4";

        public PaymentsController(IPaymentService paymentService, IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }
        [Authorize]
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{Basketid}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateorUpdatePaymentIntent(string Basketid)
        {
            var CustomerBasket = await _paymentService.CrreateOrUpdatePaymentIntent(Basketid);
            if (CustomerBasket == null) return BadRequest(new ApiResponse(400, "There is a problem with your basket"));
            var MappedBasket = _mapper.Map<CustomerBasket, CustomerBasketDto>(CustomerBasket);
            return Ok(MappedBasket);
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebHook()
        {

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);
                var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    await _paymentService.UpdatePaymentIntentTosuccedOrFailed(PaymentIntent.Id, false);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await _paymentService.UpdatePaymentIntentTosuccedOrFailed(PaymentIntent.Id, true);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
