using Microsoft.AspNetCore.Mvc;
using SimpleAuthApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace SimpleAuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [Authorize]
        [HttpPost("login")]
        public IActionResult Login([FromBody] clsRequest request)
        {
            if (request == null)
                return BadRequest("Invalid request");

            // Dummy validation
            if (request.Username == "admin" && request.Password == "1234")
            {
                return Ok("Login Success");
            }

            return Unauthorized("Invalid username or password");
        }
    }
}
