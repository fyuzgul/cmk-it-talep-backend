using CMKITTalep.API.Attributes;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CMKITTalep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [RequireAuthenticated]
    public class InventoryController : BaseController<Inventory>
    {
        public InventoryController(IInventoryService inventoryService) : base(inventoryService)
        {
        }

        [RequireSupport]
        public override async Task<ActionResult<IEnumerable<Inventory>>> GetAll()
        {
            return await base.GetAll();
        }

        [RequireSupport]
        public override async Task<ActionResult<Inventory>> GetById(int id)
        {
            return await base.GetById(id);
        }

        [RequireSupport]
        public override async Task<ActionResult<Inventory>> Create(Inventory entity)
        {
            return await base.Create(entity);
        }

        [RequireSupport]
        public override async Task<IActionResult> Update(int id, Inventory entity)
        {
            return await base.Update(id, entity);
        }

        [RequireSupport]
        public override async Task<IActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }
    }
}
