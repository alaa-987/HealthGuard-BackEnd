using AutoMapper;
using HealthGuard.Core.Entities;
using HealthGuard.Core.Repository.contract;
using HealthGuard.GradProject.DTO;
using HealthGuard.GradProject.Errors;
using HealthGurad.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthGuard.GradProject.Controllers
{
   
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepo, IMapper mapper)
        {
            _basketRepo = basketRepo;
            _mapper = mapper;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {
            var basket = await _basketRepo.GetBasketAsync(id);
            return Ok(basket ?? new CustomerBasket(id));
        }
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var creativeUpdated = await _basketRepo.UpdateBasketAsync(mappedBasket);
            if (creativeUpdated is null) return BadRequest(new ApiResponse(400));
            return Ok(creativeUpdated);
        }
        [HttpDelete]
        public async Task DeleteBasket(string id)
        {
            await _basketRepo.DeleteBasketAsync(id);
        }
        [HttpPost("add-to-basket")]
        public async Task<ActionResult<CustomerBasketDto>> AddToBasket(string basketId, BasketItemDto itemDto)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId) ?? new CustomerBasket(basketId);

            var newItem = new BasketItem
            {
                Id = itemDto.Id,
                ProductName = itemDto.ProductName,
                PicUrl = itemDto.PicUrl,
                Price = itemDto.Price,
                Category = itemDto.Category,
                Quanntity = itemDto.Quanntity
            };

            basket.Items.Add(newItem);
            var updatedBasket = await _basketRepo.UpdateBasketAsync(basket);

            var basketDto = _mapper.Map<CustomerBasketDto>(updatedBasket);
            return Ok(basketDto);
        }
        [HttpDelete("delete-from-basket")]
        public async Task<ActionResult<CustomerBasketDto>> DeleteFromBasket(string basketId, int itemId)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);
            if (basket == null)
            {
                return NotFound(new ApiResponse(404));
            }

            var itemToRemove = basket.Items.FirstOrDefault(item => item.Id == itemId);
            if (itemToRemove == null)
            {
                return NotFound(new ApiResponse(404));
            }

            basket.Items.Remove(itemToRemove);

            var updatedBasket = await _basketRepo.UpdateBasketAsync(basket);

            var basketDto = _mapper.Map<CustomerBasketDto>(updatedBasket);
            return Ok(basketDto);
        }
        [HttpPut("update-basket-item-quantity")]
        public async Task<ActionResult<CustomerBasketDto>> UpdateBasketItemQuantity(string basketId, int itemId, int newQuantity)
        {
            var basket = await _basketRepo.GetBasketAsync(basketId);
            if (basket == null)
            {
                return NotFound(new ApiResponse(404)); 
            }

            var itemToUpdate = basket.Items.FirstOrDefault(item => item.Id == itemId);
            if (itemToUpdate == null)
            {
                return NotFound(new ApiResponse(404));
            }

            itemToUpdate.Quanntity = newQuantity;

            var updatedBasket = await _basketRepo.UpdateBasketAsync(basket);

            var basketDto = _mapper.Map<CustomerBasketDto>(updatedBasket);
            return Ok(basketDto);
        }



    }
}
