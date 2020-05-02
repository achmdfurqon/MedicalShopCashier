using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity, TRepository> : ControllerBase
        where TEntity : class, IEntity
        where TRepository : IRepository<TEntity>
    {
        private readonly TRepository repository;

        public BaseController(TRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var get = await repository.GetAsync();
            get = get.Where(e => e.IsDeleted.Equals(false));
            return Ok(get);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var get = await repository.GetAsync(id);
            if (get == null)
            {
                return NotFound(get);
            }
            return Ok(get);
        }

        [HttpPost]
        public async Task<IActionResult> Post(TEntity entity)
        {
            var get = await repository.GetAsync();
            get = get.Where(e => e.Name.Equals(entity.Name));
            if (get.Count() == 0)
            {
                entity.CreateDate = DateTime.Now;
                var data = await repository.PostAsync(entity);
                if (data > 0)
                {
                    entity.Id = data;
                    return Ok(entity);
                }
            }            
            return BadRequest("Create Failed");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, TEntity entity)
        {
            var put = await repository.GetAsync(id);
            if (put != null || !put.IsDeleted)
            {
                entity.Id = id;
                entity.CreateDate = put.CreateDate;
                entity.UpdateDate = DateTime.Now;
                var isSuccess = await repository.PullAsync(entity);
                if (isSuccess)
                {
                    return Ok(entity);
                }
            }
            return BadRequest("Update Failed.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await repository.GetAsync(id);
            if (entity != null)
            {
                var isSuccess = await repository.DeleteAsync(entity);
                if (isSuccess)
                {
                    return Ok(entity);
                }
            }
            return BadRequest("Delete Failed.");
        }

        [HttpGet("Remove/{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var entity = await repository.GetAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                entity.DeleteDate = DateTime.Now;
                var isSuccess = await repository.PullAsync(entity);
                if (isSuccess)
                {
                    return Ok(entity);
                }
            }            
            return BadRequest("Remove Failed.");
        }
    }
}