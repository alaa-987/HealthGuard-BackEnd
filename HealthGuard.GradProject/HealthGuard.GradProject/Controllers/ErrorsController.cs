using HealthGuard.GradProject.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HealthGuard.GradProject.Controllers
{
    [Route("Errors/{code}")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        public ActionResult Errors(int code)
        {
            if (code == 401)
            {
                return Unauthorized(new ApiResponse(401));
            }
            else if (code == 404)
            {
                return NotFound(new ApiResponse(404));
            }
            return StatusCode(code);

        }
    }
}
