
using WebAPI.Models;

namespace NoorCare.Repository
{

    public interface IClientDetailRepository : IRepository<ClientDetail, int>
    {
    }

    public class ClientDetailRepository : EFRepositoryBase<ApplicationDbContext, ClientDetail, int>, IClientDetailRepository
    {

    }
}
