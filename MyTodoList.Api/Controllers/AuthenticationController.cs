using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyTodoList.Api.Authentication.Models;
using MyTodoList.Api.Services;
using MyTodoList.Data.Models;

namespace MyTodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;

        public AuthenticationController(UserManager<User> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new User
            {
                UserName = model.UserName,
                NormalizedUserName = model.UserName.ToUpper(),
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                string token = _jwtService.Generate(user);
                return Ok(new { Token = token });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                return Unauthorized("Incorrect user name");
            }
            var result = await _userManager.CheckPasswordAsync(user, model.Password);
            if (result)
            {
                string token = _jwtService.Generate(user);
                return Ok(new { Token = token });
            }
            return Unauthorized("Incorrect password");
        }
    }
}
