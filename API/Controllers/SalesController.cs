using System;
using System.Collections.Generic;
using Data.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data.Models;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly ISaleRepository orders;
        public SalesController(ISaleRepository repository)
        {
            orders = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await orders.GetOrders();
            result = result.Where(r => r.Status.Equals(false));
            return Ok(result);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            var result = await orders.GetOrders();
            return Ok(result.Select(s => new Sale { Product = s.Product, Quantity = s.Quantity }).GroupBy(g => g.Product));
        }

        [HttpPost]
        public IActionResult Post(SaleVM sale)
        {
            var result = orders.AddOrder(sale);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = orders.CancelOrder(id);
            return Ok(result);
        }
    }
}