using AutoMapper;
using HealthGuard.Core.Entities.Identity;
using HealthGuard.Core.Repository.contract;
using HealthGuard.GradProject.DTO;
using HealthGuard.GradProject.Errors;
using HealthGuard.GradProject.Extension;
using HealthGuard.Service.EmailService;
using HealthGurad.Repository.Data.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthGuard.GradProject.Controllers
{
   
    public class NurseController : BaseApiController
    {
        private readonly INurseRepository _nurseRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailservice;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppIDentityDbContext _dbContext;

        public NurseController(INurseRepository nurseRepository,IMapper mapper,IEmailService emailservice,UserManager<AppUser> userManager, AppIDentityDbContext dbContext)
        {
            _nurseRepository = nurseRepository;
            _mapper = mapper;
            _emailservice = emailservice;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        [HttpGet("All-Nurses")]
        public async Task<ActionResult<IEnumerable<AppNurse>>> GetNurses()
        {
            var nurses = await _nurseRepository.GetAllAsync();
            var nurseDtos = _mapper.Map<IEnumerable<AppNurse>, IEnumerable<NurseDto>>(nurses);

            return Ok(new
            {
                Count = nurseDtos.Count(),
                Nurses = nurseDtos
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NurseDto>> GetNurse(string id)
        {
            var nurse = await _nurseRepository.GetAsync(id);
            if (nurse == null)
            {
                return NotFound(new ApiResponse(404, "Nurse not found"));
            }

            var nurseDto = _mapper.Map<AppNurse, NurseDto>(nurse);
            return Ok(nurseDto);
        }


        [HttpPost("FirstCreate")]
        public async Task<ActionResult<AppNurse>> CreateNurse(AppNurse nurse)
        {
            await _nurseRepository.Add(nurse);
            return CreatedAtAction(nameof(GetNurse), new { id = nurse.Id }, nurse);
        }

        [HttpPut("FirstUpdate/{id}")]
        public async Task<IActionResult> UpdateNurse(string id, AppNurse nurse)
        {
            if (id != nurse.Id)
            {
                return BadRequest();
            }

            _nurseRepository.Update(nurse);
            return NoContent();
        }
        [HttpPost("test-email")]
        public async Task<ActionResult> TestEmail()
        {
            var result = await _emailservice.SendEmailAsync("test-recipient@example.com", "Test Subject", "Test Body");
            if (result)
            {
                return Ok("Email sent successfully");
            }
            else
            {
                return StatusCode(500, "Failed to send email");
            }
        }

        [HttpPost("book-appointment/{appNurseId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> BookAppointment(string appNurseId, [FromBody] AppointmentDto appointmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentDateTime = DateTime.UtcNow;
            if (appointmentDto.StartTime < currentDateTime || appointmentDto.EndTime < currentDateTime)
            {
                return BadRequest(new ApiResponse(400, "Appointments cannot be booked in the past."));
            }

            var nurse = await _nurseRepository.GetAsync(appNurseId);
            if (nurse == null)
            {
                return NotFound(new ApiResponse(404, "Nurse not found"));
            }

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest(new ApiResponse(400, "User email not found"));
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return BadRequest(new ApiResponse(400, "User not found"));
            }

            var existingAppointment = await _nurseRepository.GetAppointmentsAsync(appNurseId, appointmentDto.StartTime, appointmentDto.EndTime);
            if (existingAppointment != null)
            {
                var availableSlots = await GetAvailableSlotsForNurse(appNurseId);
                return BadRequest(new ApiResponse(400, "This appointment slot is already booked. Available slots: " + string.Join(", ", availableSlots)));
            }

            var appointment = new Appointment
            {
                StartTime = appointmentDto.StartTime,
                EndTime = appointmentDto.EndTime,
                PatientName = appointmentDto.PatientName,
                Street = appointmentDto.Street,
                City = appointmentDto.City,
                AppNurseId = appNurseId,
                AppNurse = nurse,
                AppUserId = user.Id,
                User = user
            };

            await _nurseRepository.AddAppointmentAsync(appointment);

            try
            {
                var emailResult = await _emailservice.SendEmailAsync(userEmail, "Appointment Confirmation",
                    $"Dear {appointmentDto.PatientName},<br>Your appointment with nurse {nurse.NurseName} has been confirmed.<br><br>Thank you!<br><br>Your contact number is: {user.PhoneNumber}");

                if (!emailResult)
                {
                    Console.WriteLine($"Failed to send confirmation email to {userEmail}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while sending email: {ex}");
            }

            return Ok(new { AppointmentId = appointment.Id, Message = "Appointment booked successfully." });
        }

        private async Task<List<string>> GetAvailableSlotsForNurse(string appNurseId)
        {
        
            return await Task.FromResult(new List<string> { "10:00 AM - 11:00 AM", "11:00 AM - 12:00 PM" });
        }

        [HttpPost("clear-appointments")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ClearAppointments()
        {
            await _nurseRepository.ClearAllAppointmentsAsync();
            return Ok(new ApiResponse(200, "All appointments have been cleared."));
        }

        [HttpGet("booking-info/{appointmentId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetBookingInfo(int appointmentId)
        {

            var appointment = await _nurseRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound(new ApiResponse(404, "Appointment not found"));
            }

            var nurse = await _nurseRepository.GetAsync(appointment.AppNurseId);
            if (nurse == null)
            {
                return NotFound(new ApiResponse(404, "Nurse not found"));
            }

            var bookingTime = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
            var bookingInfo = new
            {
                NurseName = nurse.NurseName,
                PatientName = appointment.PatientName,
                BookingDate = bookingTime,
                StreetAddress = appointment.Street,
                NurseFees = nurse.Price
            };

            return Ok(bookingInfo);
        }
        [HttpGet("all-appointments")]
        public async Task<ActionResult<IEnumerable<AllAppointmentDto>>> GetAllAppointments()
        {
            try
            {
                var appointments = await _nurseRepository.GetAllAppointmentsAsync();
                var appointmentDtos = _mapper.Map<IEnumerable<AllAppointmentDto>>(appointments);
                return Ok(appointmentDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching appointments: {ex}");
                return StatusCode(500, new ApiResponse(500, "Internal server error"));
            }
        }
        [HttpGet("my-appointments")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IEnumerable<AppointmentNurseDto>>> GetMyAppointments()
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail))
                {
                    return BadRequest(new ApiResponse(400, "User email not found"));
                }

                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    return NotFound(new ApiResponse(404, "User not found"));
                }

                var appointments = await _nurseRepository.GetAppointmentsForUserAsync(user.Id);

                var appointmentDetails = new List<AppointmentNurseDto>();
                foreach (var appointment in appointments)
                {
                    var nurse = await _nurseRepository.GetAsync(appointment.AppNurseId);
                    if (nurse != null)
                    {
                        var bookingTime = appointment.StartTime;
                        var appointmentDetail = new AppointmentNurseDto
                        {
                            AppointmentId = appointment.Id,
                            NurseName = nurse.NurseName,
                            PatientName = appointment.PatientName,
                            BookingDate = bookingTime,
                            StreetAddress = appointment.Street,
                            NurseFees = nurse.Price
                        };
                        appointmentDetails.Add(appointmentDetail);
                    }
                }

                return Ok(appointmentDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
        [HttpDelete("cancel-appointment/{appointmentId}")]
        public async Task<ActionResult> CancelAppointment(int appointmentId)
        {
            var appointment = await _nurseRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound(new ApiResponse(404, "Appointment not found"));
            }

            await _nurseRepository.RemoveAppointmentAsync(appointmentId);

            return Ok(new { Message = "Appointment cancelled successfully." });
        }
        [HttpPost("create-nurse")]
        public async Task<ActionResult<NurseDto>> CreateNurse(CreateNurseDto createNurseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (CheckEmailExists(createNurseDto.Email).Result.Value)
                return BadRequest(new ApiValidationErrorResponse() { Errors = new string[] { "This Email is already Exist" } });
            var nurse = _mapper.Map<AppNurse>(createNurseDto);
            nurse.UserName = createNurseDto.Email;
            nurse.Email = createNurseDto.Email;
            nurse.DisplayName = createNurseDto.NurseName;

            var result = await _userManager.CreateAsync(nurse, createNurseDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Failed to create nurse"));
            }

            await _userManager.AddToRoleAsync(nurse, "Nurse");

            var nurseDto = _mapper.Map<NurseDto>(nurse);
            return CreatedAtAction(nameof(GetNurse), new { id = nurse.Id }, nurseDto);
        }
        [HttpGet("NurseemailExist")]
        public async Task<ActionResult<bool>> CheckEmailExists(string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNurse(string id)
        {
            var nurse = await _nurseRepository.GetAsync(id);
            if (nurse == null)
            {
                return NotFound(new ApiResponse(404, "Nurse not found"));
            }

            await _nurseRepository.Delete(nurse);
            return Ok(new { Message = "Nurse Deleted successfully." });
        }
        [HttpPut("Nurses/{id}")]
        public async Task<IActionResult> UpdateNurse(Guid id, [FromBody] NurseUpdateDto nurseDto)
        {
            if (id != nurseDto.Id)
            {
                return BadRequest(new ApiResponse(400, "ID mismatch"));
            }

            var existingNurse = await _nurseRepository.GetAsync(id.ToString());
            if (existingNurse == null)
            {
                return NotFound(new ApiResponse(404, "Nurse not found"));
            }
            existingNurse.NurseName = nurseDto.NurseName;
            existingNurse.Price = nurseDto.Price;
            existingNurse.Description = nurseDto.Description;
            existingNurse.PicUrl = nurseDto.PicUrl;
            existingNurse.Hospital = nurseDto.Hospital;
            existingNurse.Specialty = nurseDto.Specialty;

            _nurseRepository.Update(existingNurse);

            var user = await _userManager.FindByEmailAsync(nurseDto.Email);
            if (user != null)
            {
                user.DisplayName = nurseDto.NurseName; 
                await _userManager.UpdateAsync(user);
            }

            await _dbContext.SaveChangesAsync();

            return Ok(new { Message = "Nurse Updated successfully." });
        }
        [HttpGet("appointments/{appointmentId}")]
        public async Task<ActionResult<AllAppointmentDto>> GetAppointmentById(int appointmentId)
        {
            var appointment = await _nurseRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound(new ApiResponse(404, "There Is No Appointment By This Id"));
            }

            var appointmentDto = _mapper.Map<AllAppointmentDto>(appointment);

            return Ok(appointmentDto);
        }

    }
}
