using NoorCare.Repository;
using System.Web.UI.WebControls;
using WebAPI.Entity;

namespace WebAPI.Repository
{
    public interface IClientDetailRepository : IRepository<ClientDetail, int> { }
    public interface IFacilityRepository : IRepository<Facility, int> { }
    public interface IDiseaseRepository : IRepository<Disease, int> { }
    public interface IEmergencyContactRepository : IRepository<EmergencyContact, int> { }
    public interface IMedicalInformationRepository : IRepository<MedicalInformation, int> { }
    public interface ICountryCodeRepository : IRepository<CountryCode, int> { }
    public interface ICityRepository : IRepository<TblCity, int> { }
    public interface ICountryRepository : IRepository<TblCountry, int> { }
    public interface IStateRepository : IRepository<State, int> { }
    public interface IInsuranceInformationRepository : IRepository<InsuranceInformation, int> { }
    public interface IQuickHealthRepository : IRepository<QuickHeathDetails, int> { }
    public interface IQuickUploadRepository : IRepository<QuickUpload, int> { }
    public interface IDoctorRepository : IRepository<Doctor, int> { }
    public interface ISecretaryRepository : IRepository<Secretary, int> { }
    public interface IHospitalDetailsRepository : IRepository<HospitalDetails, int> { }
    public interface IFeedbackRepository : IRepository<Feedback, int> { }
    public interface IAppointmentRepository : IRepository<Appointment, int> { }
    public interface ITblHospitalAmenitiesRepository : IRepository<TblHospitalAmenities, int> { }
    public interface ITblHospitalServicesRepository : IRepository<TblHospitalServices, int> { }
    public interface ITblHospitalSpecialtiesRepository : IRepository<TblHospitalSpecialties, int> { }
    public interface IDoctorAvailableTimeRepository : IRepository<DoctorAvailableTime, int> { }
    public interface IContactUsRepository : IRepository<ContactUs, int> { }
    public interface ITimeMasterRepository : IRepository<TimeMaster, int> { }
    public interface IPatientPrescriptionRepository : IRepository<PatientPrescription, int> { }
    public interface INewsBlogsRepository : IRepository<NewsBlogs, int> { }
    
}
