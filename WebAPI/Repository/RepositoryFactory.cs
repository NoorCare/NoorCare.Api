using WebAPI.Repository;

namespace NoorCare.Repository
{
    public static class RepositoryFactory
    {

        public static TRepository Create<TRepository>(ContextTypes ctype) where TRepository : class
        {
            switch (ctype)
            {
                case ContextTypes.EntityFramework:
                    if (typeof(TRepository) == typeof(IClientDetailRepository))
                    {
                        return new ClientDetailRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IFacilityRepository))
                    {
                        return new FacilityRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IDiseaseRepository))
                    {
                        return new DiseaseRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IReportRepository))
                    {
                        return new ReportRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IEmergencyContactRepository))
                    {
                        return new EmergencyContactRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IMedicalInformationRepository))
                    {
                        return new MedicalInformationRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ICountryCodeRepository))
                    {
                        return new CountryCodeRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ICityRepository))
                    {
                        return new CityRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IStateRepository))
                    {
                        return new StateRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IInsuranceInformationRepository))
                    {
                        return new InsuranceInformationRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IQuickHealthRepository))
                    {
                        return new QuickHealthRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IHospitalDetailsRepository))
                    {
                        return new HospitalDetailsRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IQuickUploadRepository))
                    {
                        return new QuickUploadRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ICountryRepository))
                    {
                        return new CountryRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IDoctorRepository))
                    {
                        return new DoctorRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ISecretaryRepository))
                    {
                        return new SecretaryRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IFeedbackRepository))
                    {
                        return new FeedbackRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IAppointmentRepository))
                    {
                        return new AppointmentRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IAppointmentRepository))
                    {
                        return new AppointmentRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ITblHospitalAmenitiesRepository))
                    {
                        return new TblHospitalAmenitiesRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ITblHospitalServicesRepository))
                    {
                        return new TblHospitalServicesRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ITblHospitalSpecialtiesRepository))
                    {
                        return new TblHospitalSpecialtiesRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IQuickUploadRepository))
                    {
                        return new QuickUploadRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IDoctorAvailableTimeRepository))
                    {
                        return new DoctorAvailableTimeRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IContactUsRepository))
                    {
                        return new ContactUsRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ITimeMasterRepository))
                    {
                        return new TimeMasterRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IPatientPrescriptionRepository))
                    {
                        return new PatientPrescriptionRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(INewsBlogsRepository))
                    {
                        return new NewsBlogsRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IReadLikeRepository))
                    {
                        return new ReadLikeRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IFacilityDetailRepository))
                    {
                        return new FacilityDetailRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IFacilityImagesRepository))
                    {
                        return new FacilityImagesRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IHospitalDocumentsRepository))
                    {
                        return new HospitalDocumentsRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IQuickUploadAssignRepository))
                    {
                        return new QuickUploadAssignRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ILeadRepository))
                    {
                        return new LeadRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IPatientPrescriptionAssignRepository))
                    {
                        return new PatientPrescriptionAssignRepository() as TRepository;
                    }

                    if (typeof(TRepository) == typeof(IEmailNotificationsRepository))
                    {
                        return new EmailNotificationsRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IEnquiryRepository))
                    {
                        return new EnquiryRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IInsuranceMasterRepository))
                    {
                        return new InsuranceMasterRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ILikeVisitorRepository))
                    {
                        return new LikeVisitorRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(IHospitalInsuranceRepository))
                    {
                        return new HospitalInsuranceRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ILeaveMasterRepository))
                    {
                        return new LeaveMasterRepository() as TRepository;
                    }
                    if (typeof(TRepository) == typeof(ILeaveDetailRepository))
                    {
                        return new LeaveDetailRepository() as TRepository;
                    }
                    return null;
                default:
                    return null;
            }
        }
    }
}
