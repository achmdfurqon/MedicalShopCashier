using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Contexts;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = config;
        }

        #region UserRoles 
        [HttpGet]
        public ActionResult GetUsers()
        {
            var list = new List<UserVM>();
            var users = _userManager.Users.ToList();
            foreach(var user in users)
            {
                var userVM = new UserVM(user);
                var role = _userManager.GetRolesAsync(user).Result;
                userVM.Role = string.Join(',', role);
                list.Add(userVM);
            }
            return Ok(list);
        }

        [HttpGet("Roles")]
        public ActionResult Role()
        {
            var rolelist = _roleManager.Roles.ToList();
            var roles = new List<IdentityRole>();
            foreach (var _role in rolelist)
            {
                var role = _roleManager.FindByNameAsync(_role.Name).Result;
                roles.Add(role);
            }
            return Ok(roles);
        }

        [HttpGet("Role/{id}")]
        public ActionResult Role(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            var rolelist = _userManager.GetRolesAsync(user).Result;
            List<IdentityRole> roles = new List<IdentityRole>();
            foreach(var rolename in rolelist)
            {
                var role = new IdentityRole();
                role = _roleManager.FindByNameAsync(rolename).Result;
                roles.Add(role);
            }
            return Ok(roles);
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser(UserVM userVM)
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
                        await _userManager.AddToRoleAsync(user, userVM.Role);
                        return Ok(user);
                    }
                }
                return BadRequest();
            }
            catch (Exception)
            {
                throw;
            }
        }        

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var delete = await _userManager.DeleteAsync(user);
            return Ok(delete);
        }

        [HttpGet("AssignRole")]
        public async Task<ActionResult> AssignRole(string id, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if(await _userManager.IsInRoleAsync(user, role))
                {
                    return BadRequest();
                }
                await _userManager.AddToRoleAsync(user, role);
                return Ok(user);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPut("RemoveRole/{id}")]
        public async Task<ActionResult> RemoveRole(string id, IdentityRole role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                    return Ok();
                }
                return BadRequest();
            }
            catch
            {
                return BadRequest();
            }            
        }
        #endregion
    }
}