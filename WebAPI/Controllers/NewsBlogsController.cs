using NoorCare.Repository;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class NewsBlogsController : ApiController
    {
        INewsBlogsRepository _newsBlogsRepo = RepositoryFactory.Create<INewsBlogsRepository>(ContextTypes.EntityFramework);

        [Route("api/NewsBlogs/getAllNewsBlogs/{Type}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAllNewsBlogs(string Type)
        {
            var result = _newsBlogsRepo.Find(x => x.Category == Type);

            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/NewsBlogs/SaveNewBlog")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage SaveNewsBlog(NewsBlogs obj)
        {
            var _newsBlogCreated = _newsBlogsRepo.Insert(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, "Saved");
        }

        //[Route("api/NewsBlogs/Read/{UserID}/{PageID}")]
        //[HttpPut]
        //[AllowAnonymous]
        //public HttpResponseMessage Read(string UserID, string PageID)
        //{
        //    var _newsBlogCreated = _newsBlogsRepo.Insert(obj);
        //    return Request.CreateResponse(HttpStatusCode.Accepted, "Saved");
        //}
    }
}

