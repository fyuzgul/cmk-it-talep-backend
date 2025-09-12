using CMKITTalep.Entities;

namespace CMKITTalep.Business.Interfaces
{
    public interface IRequestResponseTypeService : IGenericService<RequestResponseType>
    {
        Task<RequestResponseType?> GetByNameAsync(string name);
        Task<bool> ExistsByNameAsync(string name);
    }
}
