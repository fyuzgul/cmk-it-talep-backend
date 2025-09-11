using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController<T> : ControllerBase where T : class
    {
        protected readonly IGenericService<T> _service;

        protected BaseController(IGenericService<T> service)
        {
            _service = service;
        }

        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<T>>> GetAll()
        {
            var entities = await _service.GetAllAsync();
            return Ok(entities);
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<T>> GetById(int id)
        {
            var entity = await _service.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        [HttpPost]
        public virtual async Task<ActionResult<T>> Create(T entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdEntity = await _service.AddAsync(entity);
            var idProperty = typeof(T).GetProperty("Id");
            var entityId = idProperty?.GetValue(createdEntity);
            return CreatedAtAction(nameof(GetById), new { id = entityId }, createdEntity);
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(int id, T entity)
        {
            // Check if entity has Id property and compare
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null)
            {
                var entityId = (int)idProperty.GetValue(entity);
                if (id != entityId)
                {
                    return BadRequest("ID mismatch");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _service.UpdateAsync(entity);
            }
            catch
            {
                if (!await _service.ExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _service.ExistsAsync(id))
            {
                return NotFound();
            }

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
