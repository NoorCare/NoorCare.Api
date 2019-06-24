using WebAPI.Repository;

namespace NoorCare.Repository
{
    public static class RepositoryFactory
    {

        public static TRepository Create<TRepository>(ContextTypes ctype) where TRepository: class
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
                    return null;
                default:
                    return null;
            }
        }
    }
}
