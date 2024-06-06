using AutoMapper;
using HealthGuard.Core.Entities.Identity;
using HealthGuard.Core.Repository.contract;
using HealthGuard.Core.Services.contract;
using HealthGuard.GradProject.DTO;
using HealthGuard.GradProject.Errors;
using HealthGuard.GradProject.Extension;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthGuard.GradProject.Controllers
{
    public class AccountController : BaseApiController
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IUserAppRepository _userRepository;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> SignInManager, IAuthService authService, IMapper mapper,IUserAppRepository userRepository)
        {
            _userManager = userManager;
            _signInManager = SignInManager;
            _authService = authService;
            _mapper = mapper;
            _userRepository = userRepository;
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new ApiResponse(401));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded is false)
                return Unauthorized(new ApiResponse(401));
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
          {
           if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest(new ApiValidationErrorResponse() { Errors = new string[] { "This Email is already Exist" } });
            var user = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                PhoneNumber = model.PhoneNumber,
                IsAdmin = model.IsAdmin
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded is false) return BadRequest(new ApiResponse(400));
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }
        [HttpGet("emailExist")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
            var user = await _userManager.FindByEmailAsync(email);
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindUserByEmailWithAddressAsync(User);
            return Ok(_mapper.Map<Address, AddressDto>(user.Address));
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("address")]
        public async Task<ActionResult<Address>> UpdateUserAddress(AddressDto address)
        {
            var updatedAddress = _mapper.Map<AddressDto, Address>(address);
            var user = await _userManager.FindUserByEmailWithAddressAsync(User);
            updatedAddress.Id = user.Address.Id;
            user.Address = updatedAddress;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            return Ok(address);

        }
        [HttpGet("allUsers")]
        public async Task<ActionResult<IReadOnlyCollection<AppUser>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();

            var filteredUsers = users.Where(u => !(u is AppNurse nurse && string.IsNullOrWhiteSpace(nurse.NurseName)));

            return Ok(filteredUsers);
        }
        [HttpGet("normalUsers")]
        public async Task<ActionResult<IReadOnlyCollection<AppUser>>> GetAllNormalUsers()
        {
            var users = await _userRepository.GetAllNormalUsersAsync();
            return Ok(users);
        }
        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _userRepository.DeleteUserAsync(userId);
            return Ok(new {Message ="User Deleted Successfully"});
        }
        [HttpGet("users/{userId}")]
        public async Task<ActionResult<AppUser>> GetUserById(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("updateAccount")]
        public async Task<ActionResult<UserDto>> UpdateUserAccount([FromBody] UserUpdateDto userUpdateDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found"));
            }

            // Update display name and phone number
            user.DisplayName = userUpdateDto.DisplayName ?? user.DisplayName;
            user.PhoneNumber = userUpdateDto.PhoneNumber ?? user.PhoneNumber;

            // Reset password if new password is provided
            if (!string.IsNullOrWhiteSpace(userUpdateDto.NewPassword))
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, resetToken, userUpdateDto.NewPassword);
                if (!resetPasswordResult.Succeeded)
                {
                    return BadRequest(new ApiResponse(400, "Failed to reset password"));
                }
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Failed to update user account"));
            }

            return Ok(new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordUpdateDto changePasswordDto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new ApiResponse(404, "User not found"));
            }

            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Failed to remove old password"));
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, changePasswordDto.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Failed to add new password"));
            }

            return Ok(new { Message = "Password Updated Succesfully" });
        }

        }
}
