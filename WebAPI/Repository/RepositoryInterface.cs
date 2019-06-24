using NoorCare.Repository;
using WebAPI.Entity;

namespace WebAPI.Repository
{
    public interface IClientDetailRepository : IRepository<ClientDetail, int>{}
    public interface IFacilityRepository : IRepository<Facility, int>{}
    public interface IDiseaseRepository : IRepository<Disease, int>{}
    public interface IEmergencyContactRepository : IRepository<EmergencyContact, int>{}
    public interface IMedicalInformationRepository : IRepository<MedicalInformation, int>{}
    public interface ICountryCodeRepository : IRepository<CountryCode, int> { }
    public interface ICityRepository : IRepository<City, int> { }
    public interface IStateRepository : IRepository<State, int> { }
}
