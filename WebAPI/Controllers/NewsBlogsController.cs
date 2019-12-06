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
        IReadLikeRepository _readLikeRepo = RepositoryFactory.Create<IReadLikeRepository>(ContextTypes.EntityFramework);

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

        [Route("api/NewsBlogs/Read")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Read(ReadLike obj)
        {
            var read = _readLikeRepo.Find(x => x.UserId == obj.UserId && x.IsRead == true && x.Type== obj.Type).ToList();
            if (read.Count == 0)
            {
                obj.ReadDate = System.DateTime.Now.ToString();
                _readLikeRepo.Insert(obj);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, "Saved");
        }

        [Route("api/NewsBlogs/Like")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Like(ReadLike obj)
        {
            var like = _readLikeRepo.Find(x => x.UserId == obj.UserId && x.IsLike == true && x.Type == obj.Type).ToList();
            if (like.Count==0)
            {
                _readLikeRepo.Insert(obj);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, "Saved");
        }
        [Route("api/NewsBlogs/ReadLikeCount/{Type}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage ReadLikeCount(string Type)
        {
            var likeCount = _readLikeRepo.Find(x=>x.IsLike == true && x.Type==Type).ToList().Count;
            var readCount = _readLikeRepo.Find(x=>x.IsRead == true && x.Type == Type).ToList().Count;
            var result = new { LikeCount= likeCount, ReadCount= readCount };
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }
    }
}

