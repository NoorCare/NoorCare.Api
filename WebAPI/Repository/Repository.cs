using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;

namespace NoorCare.Repository
{
    public class ClientDetailRepository : EFRepositoryBase<ApplicationDbContext, ClientDetail, int>, IClientDetailRepository{}
    public class FacilityRepository : EFRepositoryBase<ApplicationDbContext, Facility, int>, IFacilityRepository{}
    public class DiseaseRepository : EFRepositoryBase<ApplicationDbContext, Disease, int>, IDiseaseRepository{}
    public class ReportRepository : EFRepositoryBase<ApplicationDbContext, Report, int>, IReportRepository { }
    public class EmergencyContactRepository : EFRepositoryBase<ApplicationDbContext, EmergencyContact, int>, IEmergencyContactRepository{}
    public class MedicalInformationRepository : EFRepositoryBase<ApplicationDbContext, MedicalInformation, int>, IMedicalInformationRepository{}
    public class CountryCodeRepository : EFRepositoryBase<ApplicationDbContext, CountryCode, int>, ICountryCodeRepository { }
    public class CityRepository : EFRepositoryBase<ApplicationDbContext, TblCity, int>, ICityRepository { }
    public class CountryRepository : EFRepositoryBase<ApplicationDbContext, TblCountry, int>, ICountryRepository { }
    public class StateRepository : EFRepositoryBase<ApplicationDbContext, State, int>, IStateRepository { }
    public class InsuranceInformationRepository : EFRepositoryBase<ApplicationDbContext, InsuranceInformation, int>, IInsuranceInformationRepository { }
    public class QuickHealthRepository : EFRepositoryBase<ApplicationDbContext, QuickHeathDetails, int>, IQuickHealthRepository { }
    public class HospitalDetailsRepository : EFRepositoryBase<ApplicationDbContext, HospitalDetails, int>, IHospitalDetailsRepository { }
    public class QuickUploadRepository : EFRepositoryBase<ApplicationDbContext, QuickUpload, int>, IQuickUploadRepository { }
    public class DoctorRepository : EFRepositoryBase<ApplicationDbContext,Doctor,int>, IDoctorRepository { }
    public class SecretaryRepository : EFRepositoryBase<ApplicationDbContext, Secretary, int>, ISecretaryRepository { }
    public class FeedbackRepository : EFRepositoryBase<ApplicationDbContext, Feedback, int>, IFeedbackRepository { }
    public class AppointmentRepository : EFRepositoryBase<ApplicationDbContext, Appointment, int>, IAppointmentRepository { }  
    public class TblHospitalSpecialtiesRepository : EFRepositoryBase<ApplicationDbContext, TblHospitalSpecialties, int>, ITblHospitalSpecialtiesRepository { }
    public class TblHospitalServicesRepository : EFRepositoryBase<ApplicationDbContext, TblHospitalServices, int>, ITblHospitalServicesRepository { }
    public class TblHospitalAmenitiesRepository : EFRepositoryBase<ApplicationDbContext, TblHospitalAmenities, int>, ITblHospitalAmenitiesRepository { }
    public class DoctorAvailableTimeRepository : EFRepositoryBase<ApplicationDbContext, DoctorAvailableTime, int>, IDoctorAvailableTimeRepository { }
    public class ContactUsRepository : EFRepositoryBase<ApplicationDbContext, ContactUs, int>, IContactUsRepository { }
    public class TimeMasterRepository : EFRepositoryBase<ApplicationDbContext, TimeMaster, int>, ITimeMasterRepository { }
    public class PatientPrescriptionRepository : EFRepositoryBase<ApplicationDbContext, PatientPrescription, int>, IPatientPrescriptionRepository { }
    public class PatientPrescriptionAssignRepository : EFRepositoryBase<ApplicationDbContext, PatientPrescriptionAssign, int>, IPatientPrescriptionAssignRepository { }
    public class NewsBlogsRepository : EFRepositoryBase<ApplicationDbContext, NewsBlogs, int>, INewsBlogsRepository { }
    public class ReadLikeRepository : EFRepositoryBase<ApplicationDbContext, ReadLike, int>, IReadLikeRepository { }
    public class FacilityDetailRepository : EFRepositoryBase<ApplicationDbContext, FacilityDetail, int>, IFacilityDetailRepository { }
    public class FacilityImagesRepository : EFRepositoryBase<ApplicationDbContext, FacilityImages, int>, IFacilityImagesRepository { }
    public class HospitalDocumentsRepository : EFRepositoryBase<ApplicationDbContext, HospitalDocumentVerification, int>, IHospitalDocumentsRepository { }
    public class QuickUploadAssignRepository : EFRepositoryBase<ApplicationDbContext, QuickUploadAssign, int>, IQuickUploadAssignRepository { }
    public class LeadRepository : EFRepositoryBase<ApplicationDbContext, Lead, int>, ILeadRepository { }
    public class EmailNotificationsRepository : EFRepositoryBase<ApplicationDbContext, EmailNotifications, int>, IEmailNotificationsRepository { }
    public class EnquiryRepository : EFRepositoryBase<ApplicationDbContext, Enquiry, int>, IEnquiryRepository { }
    public class InsuranceMasterRepository : EFRepositoryBase<ApplicationDbContext, InsuranceMaster, int>, IInsuranceMasterRepository { }
    public class LikeVisitorRepository : EFRepositoryBase<ApplicationDbContext, LikeVisitor, int>, ILikeVisitorRepository { }
    public class HospitalInsuranceRepository : EFRepositoryBase<ApplicationDbContext, HospitalInsurance, int>, IHospitalInsuranceRepository { }

    public class LeaveDetailRepository : EFRepositoryBase<ApplicationDbContext, LeaveDetail, int>, ILeaveDetailRepository { }
    public class LeaveMasterRepository : EFRepositoryBase<ApplicationDbContext, LeaveMaster, int>, ILeaveMasterRepository { }
}


