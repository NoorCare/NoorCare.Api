
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;

namespace NoorCare.Repository
{
    public class ClientDetailRepository : EFRepositoryBase<ApplicationDbContext, ClientDetail, int>, IClientDetailRepository{}
    public class FacilityRepository : EFRepositoryBase<ApplicationDbContext, Facility, int>, IFacilityRepository{}
    public class DiseaseRepository : EFRepositoryBase<ApplicationDbContext, Disease, int>, IDiseaseRepository{}
    public class EmergencyContactRepository : EFRepositoryBase<ApplicationDbContext, EmergencyContact, int>, IEmergencyContactRepository{}
    public class MedicalInformationRepository : EFRepositoryBase<ApplicationDbContext, MedicalInformation, int>, IMedicalInformationRepository{}
    public class CountryCodeRepository : EFRepositoryBase<ApplicationDbContext, CountryCode, int>, ICountryCodeRepository { }
    public class CityRepository : EFRepositoryBase<ApplicationDbContext, TblCity, int>, ICityRepository { }
    public class CountryRepository : EFRepositoryBase<ApplicationDbContext, TblCountry, int>, ICountryRepository { }
    public class StateRepository : EFRepositoryBase<ApplicationDbContext, State, int>, IStateRepository { }
    public class InsuranceInformationRepository : EFRepositoryBase<ApplicationDbContext, InsuranceInformation, int>,
        IInsuranceInformationRepository { }
    public class QuickHealthRepository : EFRepositoryBase<ApplicationDbContext, QuickHeathDetails, int>, IQuickHealthRepository { }
    public class HospitalDetailsRepository : EFRepositoryBase<ApplicationDbContext, HospitalDetail, int>, IHospitalDetailsRepository { }
    public class QuickUploadRepository : EFRepositoryBase<ApplicationDbContext, QuickUpload, int>, IQuickUploadRepository { }
}


