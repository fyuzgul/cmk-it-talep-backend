using CMKITTalep.Business.Interfaces;
using CMKITTalep.DataAccess.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.Business.Services
{
    public class InventoryService : GenericService<Inventory>, IInventoryService
    {
        public InventoryService(IInventoryRepository repository) : base(repository)
        {
        }
    }
}
