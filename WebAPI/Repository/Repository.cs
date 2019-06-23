
using WebAPI.Entity;
using WebAPI.Models;

namespace NoorCare.Repository
{

    public interface IClientDetailRepository : IRepository<ClientDetail, int>
    {
    }

    public interface IFacilityRepository : IRepository<Facility, int>
    {
    }

    public interface IDiseaseRepository : IRepository<Disease, int>
    {
    }
    public class ClientDetailRepository : EFRepositoryBase<ApplicationDbContext, ClientDetail, int>, IClientDetailRepository
    {

    }
    public class FacilityRepository : EFRepositoryBase<ApplicationDbContext, Facility, int>, IFacilityRepository
    {

    }
    public class DiseaseRepository : EFRepositoryBase<ApplicationDbContext, Disease, int>, IDiseaseRepository
    {

    }
}


