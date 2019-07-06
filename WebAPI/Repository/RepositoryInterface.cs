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
    public interface ICityRepository : IRepository<TblCity, int> { }
    public interface ICountryRepository : IRepository<TblCountry, int> { }
    public interface IStateRepository : IRepository<State, int> { }
    public interface IInsuranceInformationRepository : IRepository<InsuranceInformation, int> { }
    public interface IQuickHealthRepository : IRepository<QuickHeathDetails, int> { }
    public interface IHospitalDetailsRepository : IRepository<HospitalDetail, int> { }
    public interface IQuickUploadRepository : IRepository<QuickUpload, int> { }
}
