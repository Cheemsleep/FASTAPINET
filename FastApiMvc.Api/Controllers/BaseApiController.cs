using FastApiMvc.Model.Dto;
using Microsoft.AspNetCore.Mvc;

namespace FastApiMvc.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "Success")
        {
            return Ok(ApiResponse<T>.Ok(data, message));
        }

        protected IActionResult Fail<T>(string message)
        {
            return Ok(ApiResponse<T>.Fail(message));
        }

        protected IActionResult NotFound<T>(string message = "Resource not found")
        {
            return Ok(ApiResponse<T>.Fail(message));
        }
    }
}
