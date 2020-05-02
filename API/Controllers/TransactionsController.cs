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
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transaction;
        public TransactionsController(ITransactionRepository repository)
        {
            _transaction = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _transaction.GetTransactions();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var result = _transaction.GetTransaction(id);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult Post(Transaction transaction)
        {
            var result = _transaction.AddTransaction(transaction);
            return Ok(result);
        }
    }
}