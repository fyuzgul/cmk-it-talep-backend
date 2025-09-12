using CMKITTalep.Entities;

namespace CMKITTalep.DataAccess.Interfaces
{
    public interface IRequestResponseTypeRepository : IGenericRepository<RequestResponseType>
    {
        Task<RequestResponseType?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
