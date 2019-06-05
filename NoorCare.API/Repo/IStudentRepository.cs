using RepositoryPattern.Core.DomainObjecs;

namespace RepositoryPattern.Repository
{
    public interface IStudentRepository: IRepository<Student, int>
    {
    }
}
