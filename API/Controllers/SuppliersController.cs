using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : BaseController<Supplier, SupplierRepository>
    {
        private readonly SupplierRepository supplier;
        public SuppliersController(SupplierRepository repository) : base(repository)
        {
            supplier = repository;
        }

        [HttpGet("Data")]
        public async Task<ActionResult> GetData(string keyword, int page, int size)
        {
            if (keyword == null)
            {
                keyword = "";
            }
            var data = await supplier.Get(keyword, page, size);
            if (data != null)
            {
                return Ok(data);
            }
            return BadRequest();
        }
    }
}