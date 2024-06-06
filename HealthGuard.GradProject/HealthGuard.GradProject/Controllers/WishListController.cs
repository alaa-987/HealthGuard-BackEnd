using AutoMapper;
using HealthGuard.Core.Entities;
using HealthGuard.Core.Repository.contract;
using HealthGuard.GradProject.DTO;
using HealthGuard.GradProject.Errors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthGuard.GradProject.Controllers
{
   
    public class WishListController : BaseApiController
    {
       
        private readonly IWishListRepository _wishListRepository;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Product> _productRepo;

        public WishListController(IWishListRepository wishListRepository, IMapper mapper, IGenericRepository<Product> productRepo)
        {
            _wishListRepository = wishListRepository;
            _mapper = mapper;
            _productRepo = productRepo;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<object>> GetWishList()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var wishlistId = $"user_{userId}_wishlist";
                var wishList = await _wishListRepository.GetWishlistAsync(wishlistId);

                if (wishList == null)
                {
                    wishList = new CustomerWishList(wishlistId);
                    await _wishListRepository.CreateOrUpdateWishListAsync(wishList);
                }

                var totalPrice = wishList.Items.Sum(item => item.Price * item.Quanntity);
                var totalQuantity = wishList.Items.Sum(item => item.Quanntity);
                var wishlistdto = _mapper.Map<CustomerWishlistDto>(wishList);

                var response = new
                {
                    WishList = wishlistdto,
                    Message = "WishList retrieved successfully."
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<ActionResult<object>> UpdateWishList(CustomerWishlistDto Wishlist)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var wishlistId = $"user_{userId}_wishlist";
                var mappedWishlist = _mapper.Map<CustomerWishlistDto, CustomerWishList>(Wishlist);

                mappedWishlist.Id = wishlistId;

                var updatedWishLIst = await _wishListRepository.UpdateWishlistAsync(mappedWishlist);

                //var totalPrice = updatedWishLIst.Items.Sum(item => item.Price * item.Quanntity);
                //var totalQuantity = updatedBasket.Items.Sum(item => item.Quanntity);

                var WishListDto = _mapper.Map<CustomerWishlistDto>(updatedWishLIst);

                var response = new
                {
                    WishList = WishListDto,
                    Message = "WishList updated successfully."
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
        public async Task<ActionResult> DeleteWishList()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var WishListId = $"user_{userId}_wishlist";

                await _wishListRepository.DeleteWishListAsync(WishListId);

                return Ok(new { Message = "wishlist deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("add-to-wishlist")]
        public async Task<ActionResult<object>> AddToWishlist([FromBody] ProductIdDto productIdDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var wishlistId = $"user_{userId}_wishlist";
                var Wishlist = await _wishListRepository.GetWishlistAsync(wishlistId) ?? new CustomerWishList(wishlistId);

                var existingItem = Wishlist.Items.FirstOrDefault(item => item.Id == productIdDto.ProductId);
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

                    var newItem = new WishListItem
                    {
                        Id = product.Id,
                        ProductName = product.Name,
                        PicUrl = $"https://localhost:7249/{product.PictureUrl}",
                        Price = product.Price,
                        Category = product.Category?.Name ?? "Uncategorized",
                        Quanntity = 1
                    };

                    Wishlist.Items.Add(newItem);
                }

                var updatedWishList = await _wishListRepository.UpdateWishlistAsync(Wishlist);

                //var totalPrice = updatedBasket.Items.Sum(item => item.Price * item.Quanntity);
                //var totalQuantity = updatedBasket.Items.Sum(item => item.Quanntity);
                var WishListDto = _mapper.Map<CustomerWishlistDto>(updatedWishList);

                var response = new
                {
                    WishList = WishListDto,
                    Message = $"Product with ID '{productIdDto}' added to the basket.",
                    //TotalPrice = totalPrice,
                    //TotalQuantity = totalQuantity
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("delete-from-wishlsit/{itemId}")]
        public async Task<ActionResult<object>> DeleteFromWishList(int itemId)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var WishListId = $"user_{userId}_wishlist";

                var Wishlist = await _wishListRepository.GetWishlistAsync(WishListId);
                if (Wishlist == null)
                {
                    return NotFound(new ApiResponse(404));
                }

                var itemToRemove = Wishlist.Items.FirstOrDefault(item => item.Id == itemId);
                if (itemToRemove == null)
                {
                    return NotFound(new ApiResponse(404));
                }

                Wishlist.Items.Remove(itemToRemove);

                var updatedWishList = await _wishListRepository.UpdateWishlistAsync(Wishlist);

                //var totalPrice = updatedBasket.Items.Sum(item => item.Price * item.Quanntity);
                //var totalQuantity = updatedBasket.Items.Sum(item => item.Quanntity);

                var WishListDto = _mapper.Map<CustomerWishlistDto>(updatedWishList);

                var response = new
                {
                    WishList = WishListDto,
                    Message = $"Product '{itemToRemove.ProductName}' removed from the basket.",
                    //TotalPrice = totalPrice,
                    //TotalQuantity = totalQuantity
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Internal Server Error", Error = ex.Message });
            }
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("update-WishList-item-quantity/{itemId}")]
        public async Task<ActionResult<object>> UpdateWishlistItemQuantity(int itemId, [FromBody] QuantityDto newQuantity)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var WishListId = $"user_{userId}_wishlist";

                var Wishlist = await _wishListRepository.GetWishlistAsync(WishListId);
                if (Wishlist == null)
                {
                    return NotFound(new ApiResponse(404));
                }

                var itemToUpdate = Wishlist.Items.FirstOrDefault(item => item.Id == itemId);
                if (itemToUpdate == null)
                {
                    return NotFound(new ApiResponse(404));
                }

                itemToUpdate.Quanntity = newQuantity.Count;

                var updatedWishlist = await _wishListRepository.UpdateWishlistAsync(Wishlist);

                //var totalPrice = updatedBasket.Items.Sum(item => item.Price * item.Quanntity);
                //var totalQuantity = updatedBasket.Items.Sum(item => item.Quanntity);

                var wishlistDto = _mapper.Map<CustomerWishlistDto>(updatedWishlist);

                var response = new
                {
                    WishList = wishlistDto,
                    Message = $"Quantity of product '{itemToUpdate.ProductName}' updated to {newQuantity.Count}.",
                    //TotalPrice = totalPrice,
                    //TotalQuantity = totalQuantity
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
