using AutoMapper;
using HealthGuard.Core.Entities;
using HealthGuard.Core.Repository.contract;
using HealthGuard.GradProject.DTO;
using HealthGuard.GradProject.Errors;
using HealthGurad.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthGuard.GradProject.Controllers
{
   
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Product> _productRepo;

        public BasketController(IBasketRepository basketRepo, IMapper mapper,IGenericRepository<Product> productRepo)
        {
            _basketRepo = basketRepo;
            _mapper = mapper;
            _productRepo = productRepo;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<object>> GetBasket()
        {
            try
            {
                var basketIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "BasketId");
                string basketId = basketIdClaim?.Value ?? "user_default_cart";

                var basket = await _basketRepo.GetBasketAsync(basketId);

                if (basket == null || !basket.Items.Any())
                {
                    basket = new CustomerBasket(basketId);
                    await _basketRepo.CreateOrUpdateBasketAsync(basket);
                }

                var totalPrice = basket.Items.Sum(item => item.Price * item.Quanntity);
                var totalQuantity = basket.Items.Sum(item => item.Quanntity);
                var basketDto = _mapper.Map<CustomerBasketDto>(basket);

                var response = new
                {
                    Basket = basketDto,
                    Message = "Basket retrieved successfully.",
                    TotalPrice = totalPrice,
                    TotalQuantity = totalQuantity
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving basket: {ex.Message}");
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult<object>> UpdateBasket(CustomerBasketDto basket)
        {
            try
            {
                var basketIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "BasketId");
                string basketId = basketIdClaim?.Value ?? "user_default_cart";
                var mappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);

                mappedBasket.Id = basketId;

                var updatedBasket = await _basketRepo.UpdateBasketAsync(mappedBasket);

                var totalPrice = updatedBasket.Items.Sum(item => item.Price * item.Quanntity);
                var totalQuantity = updatedBasket.Items.Sum(item => item.Quanntity);

                var basketDto = _mapper.Map<CustomerBasketDto>(updatedBasket);

                var response = new
                {
                    Basket = basketDto,
                    Message = "Basket updated successfully.",
                    TotalPrice = totalPrice,
                    TotalQuantity = totalQuantity
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBasket()
        {
            try
            {
                var basketIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "BasketId");
                string basketId = basketIdClaim?.Value ?? "user_default_cart";
                await _basketRepo.DeleteBasketAsync(basketId);

                return Ok(new { Message = "Basket deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("add-to-basket")]
        public async Task<ActionResult<object>> AddToBasket([FromBody] ProductIdDto productIdDto)
        {
            try
            {
                var basketIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "BasketId");
                string basketId = basketIdClaim?.Value ?? "user_default_cart";
                var basket = await _basketRepo.GetBasketAsync(basketId) ?? new CustomerBasket(basketId);

                var existingItem = basket.Items.FirstOrDefault(item => item.Id == productIdDto.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quanntity++;
                }
                else
                {
                    var product = await _productRepo.GetAsync(productIdDto.ProductId);
                    if (product == null)
                    {
                        return NotFound(new { Message = $"Product with ID '{productIdDto.ProductId}' not found." });
                    }

                    var newItem = new BasketItem
                    {
                        Id = product.Id,
                        ProductName = product.Name,
                        PicUrl = $"https://localhost:7249/{product.PictureUrl}",
                        Price = product.Price,
                        Category = product.Category?.Name ?? "Uncategorized",
                        Quanntity = 1
                    };

                    basket.Items.Add(newItem);
                }

                var updatedBasket = await _basketRepo.UpdateBasketAsync(basket);

                var totalPrice = updatedBasket.Items.Sum(item => item.Price * item.Quanntity);
                var totalQuantity = updatedBasket.Items.Sum(item => item.Quanntity);
                var basketDto = _mapper.Map<CustomerBasketDto>(updatedBasket);

                var response = new
                {
                    Basket = basketDto,
                    Message = $"Product with ID '{productIdDto}' added to the basket.",
                    TotalPrice = totalPrice,
                    TotalQuantity = totalQuantity
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }



        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("delete-from-basket/{itemId}")]
        public async Task<ActionResult<object>> DeleteFromBasket(int itemId)
        {
            try
            {
                var basketIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "BasketId");
                string basketId = basketIdClaim?.Value ?? "user_default_cart";

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

                var totalPrice = updatedBasket.Items.Sum(item => item.Price * item.Quanntity);
                var totalQuantity = updatedBasket.Items.Sum(item => item.Quanntity);

                var basketDto = _mapper.Map<CustomerBasketDto>(updatedBasket);

                var response = new
                {
                    Basket = basketDto,
                    Message = $"Product '{itemToRemove.ProductName}' removed from the basket.",
                    TotalPrice = totalPrice,
                    TotalQuantity = totalQuantity
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("update-basket-item-quantity/{itemId}")]
        public async Task<ActionResult<object>> UpdateBasketItemQuantity(int itemId, [FromBody] QuantityDto newQuantity)
        {
            try
            {
                var basketIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "BasketId");
                string basketId = basketIdClaim?.Value ?? "user_default_cart";

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

                itemToUpdate.Quanntity = newQuantity.Count;

                var updatedBasket = await _basketRepo.UpdateBasketAsync(basket);

                var totalPrice = updatedBasket.Items.Sum(item => item.Price * item.Quanntity);
                var totalQuantity = updatedBasket.Items.Sum(item => item.Quanntity);

                var basketDto = _mapper.Map<CustomerBasketDto>(updatedBasket);

                var response = new
                {
                    Basket = basketDto,
                    Message = $"Quantity of product '{itemToUpdate.ProductName}' updated to {newQuantity.Count}.",
                    TotalPrice = totalPrice,
                    TotalQuantity = totalQuantity
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }



    }
}
