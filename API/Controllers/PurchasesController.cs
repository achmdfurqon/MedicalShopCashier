using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Interfaces;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseRepository purchases;
        public PurchasesController(IPurchaseRepository repository)
        {
            purchases = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await purchases.GetPurchases();
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post(PurchaseVM purchase)
        {
            var result = purchases.AddPurchase(purchase);
            return Ok(result);
        }
    }
}