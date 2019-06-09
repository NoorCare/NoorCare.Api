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
                    return null;
                default:
                    return null;
            }
        }
    }
}
