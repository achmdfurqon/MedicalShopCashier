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
    public class ProductsController : BaseController<Product, ProductRepository>
    {
        private readonly ProductRepository product;
        public ProductsController(ProductRepository repository) : base(repository) 
        {
            product = repository;
        }

        [HttpGet("Data")]
        public async Task<ActionResult<DataTableProducts>> GetData(string keyword, int page, int size)
        {
            if (keyword == null) { keyword = ""; }
            var data = await product.Get(keyword, page, size);
            if (data != null)
            {
                return Ok(data);
            }
            return BadRequest();
        }
    }
}