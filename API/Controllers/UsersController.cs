using System;
using System.Collections;
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
    public class UsersController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config)
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
                        //await _userManager.AddToRoleAsync(user, "Admin");
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
                    userVM.Role = await _userManager.GetRolesAsync(user);                    
                    return Ok(userVM);
                }
                else
                {
                    var message = "Username or Password is Invalid";
                    return BadRequest(message);
                }
            }
            catch(Exception)
            {
                throw;
            }
        }

        //#region UserRoles        
        //[HttpGet("UserRole")]
        //public async Task<ActionResult> Users()
        //{
        //    var users = await _userManager.GetUsersInRoleAsync("User");
        //    return Ok(users);
        //}
        //[HttpPost("AssignRole")]
        //public async Task<ActionResult> AssignRole(IdentityUserRole<string> userRole)
        //{
        //    var user = await _userManager.FindByIdAsync(userRole.UserId);
        //    var role = await _roleManager.FindByIdAsync(userRole.RoleId);
        //    var assign = new IdentityResult();
        //    var x = await _userManager.IsInRoleAsync(user, role.Name);
        //    if (!x)
        //    {
        //        assign = await _userManager.AddToRoleAsync(user, role.Name);
        //    }
        //    return Ok(assign);
        //}
        //[HttpPost("RemoveRole")]
        //public async Task<ActionResult> RemoveRole(IdentityUserRole<string> userRole)
        //{
        //    var user = await _userManager.FindByIdAsync(userRole.UserId);
        //    var role = await _roleManager.FindByIdAsync(userRole.RoleId);
        //    var remove = new IdentityResult();
        //    var x = await _userManager.IsInRoleAsync(user, role.Name);
        //    if (x)
        //    {
        //        remove = await _userManager.RemoveFromRoleAsync(user, role.Name);
        //    }
        //    return Ok(remove);
        //}
        //#endregion
    }
}