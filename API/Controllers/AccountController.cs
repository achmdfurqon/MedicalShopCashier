using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = config;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserVM userVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User user = new User(userVM);
                    user.Id = Guid.NewGuid().ToString();
                    var result = await _userManager.CreateAsync(user, userVM.PasswordHash);
                    if (result.Succeeded)
                    {
                        return Ok(user);
                    }
                    AddErrors(result);
                }
                return BadRequest(ModelState);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // POST: api/Users
        [HttpPost("Login")]
        public async Task<ActionResult> Login(UserVM userVM)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(userVM.Email);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(userVM.Email);
                }
                var result = await _signInManager.PasswordSignInAsync(user.UserName, userVM.PasswordHash, false, true);
                if (result.Succeeded)
                {
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim("UserName", user.UserName),
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: signIn
                        );
                    userVM = new UserVM(user);
                    userVM.Token = new JwtSecurityTokenHandler().WriteToken(token);
                    var role = await _userManager.GetRolesAsync(user);
                    userVM.Role = string.Join(' ', role);
                    return Ok(userVM);
                }
                else
                {
                    var message = "Username or Password is Invalid";
                    return BadRequest(message);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("{email}")]
        public ActionResult GetUser(string email)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            var userVM = new UserVM(user);
            var role = _userManager.GetRolesAsync(user).Result;
            userVM.Role = string.Join(',', role);
            return Ok(userVM);
        }

        [HttpPut("ForgetPassword/{id}")]
        public async Task<ActionResult> ForgetPassword(string id, UserVM userVM)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                var result = await _userManager.ChangePasswordAsync(user, user.PasswordHash, userVM.PasswordHash);
                if (result.Succeeded)
                {
                    return Ok(result);
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}