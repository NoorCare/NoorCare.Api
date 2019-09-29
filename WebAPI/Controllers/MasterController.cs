using NoorCare.Repository;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class MasterController : ApiController
    {
        ITimeMasterRepository _timeMasterRepo = RepositoryFactory.Create<ITimeMasterRepository>(ContextTypes.EntityFramework);

        [Route("api/GetTimeMaster")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAllTime()
        {
            var result = _timeMasterRepo.GetAll().ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

      
    }
}

