using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        #region Roles
        // GET: Roles
        [HttpGet]
        public ActionResult List()
        {
            var list = _roleManager.Roles.ToList();
            return Ok(list);
        }

        // GET: Roles/Details/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Details(string id)
        {
            var detail = await _roleManager.FindByIdAsync(id);
            //var detail = await _roleManager.FindByNameAsync(id);
            return Ok(detail);
        }

        // POST: Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(IdentityRole role)
        {
            try
            {
                // TODO: Add insert logic here
                var result = new IdentityResult();
                var x = await _roleManager.RoleExistsAsync(role.Name);
                if (!x)
                {
                    var _role = new IdentityRole();
                    _role.Name = role.Name;
                    result = await _roleManager.CreateAsync(_role);
                }
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST: Roles/Edit/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Edit(string id, IdentityRole role)
        {
            try
            {
                // TODO: Add update logic here
                var result = new IdentityResult();
                var x = await _roleManager.RoleExistsAsync(role.Name);
                if (!x)
                {
                    var _role = await _roleManager.FindByIdAsync(id);
                    //var _role = await _roleManager.FindByNameAsync(id);
                    _role.Name = role.Name;
                    result = await _roleManager.UpdateAsync(_role);
                }
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST: Roles/Delete/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                // TODO: Add delete logic here
                var role = await _roleManager.FindByIdAsync(id);
                //var role = await _roleManager.FindByNameAsync(id);
                var result = await _roleManager.DeleteAsync(role);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }
        #endregion
    }
}