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


	[Route("api/NewsBlogs/GetNewsBlog/{id}")]
	[HttpGet]
	[AllowAnonymous]
	public HttpResponseMessage GetNewsBlog(int id)
	{
	    var result = _newsBlogsRepo.Get(id);

	    return Request.CreateResponse(HttpStatusCode.OK, result);
	}

	[Route("api/NewsBlogs/SaveNewBlog")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage SaveNewsBlog(NewsBlogs obj)
        {
            var _newsBlogCreated = _newsBlogsRepo.Insert(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, "Saved");
        }

        [Route("api/NewsBlogs/UpdateNewsBlog/{userid}/{id}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage UpdateNewsBlog(string userid, int id)
        {
            try
            {
                var _newsBlog = _newsBlogsRepo.Find(x => x.Id == id && x.UserId == userid && x.IsDeleted == false).FirstOrDefault();
                if (_newsBlog != null)
                {
                    return Request.CreateResponse(HttpStatusCode.Accepted, _newsBlog);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not Found");
                }
            }
            catch (System.Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("api/NewsBlogs/UpdateNewsBlog")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage UpdateNewsBlog(NewsBlogs obj)
        {
            try
            {
                string msg = string.Empty;
                var _newsBlog = _newsBlogsRepo.Find(x => x.Id == obj.Id && x.UserId == obj.UserId && x.IsDeleted == false).FirstOrDefault();
                if (_newsBlog != null)
                {
                    _newsBlog.PageId = obj.PageId;
                    _newsBlog.Category = obj.Category;
                    _newsBlog.ContentText = obj.ContentText;
                    _newsBlog.ModifiedBy = obj.UserId;
                    _newsBlog.ModifiedDate = System.DateTime.Now.ToString();
                    _newsBlogsRepo.Update(_newsBlog);
                    return Request.CreateResponse(HttpStatusCode.Accepted, "Updated");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not Found");
                }
            }
            catch (System.Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [Route("api/NewsBlogs/DeleteNewsBlog/{userid}/{id}")]
        [HttpDelete]
        [AllowAnonymous]
        public HttpResponseMessage DeleteNewsBlog(string userid, int id)
        {
            try
            {
                var _newsBlog = _newsBlogsRepo.Find(x => x.Id == id && x.UserId == userid).FirstOrDefault();
                if (_newsBlog != null)
                {
                    _newsBlog.IsDeleted = true;
                    _newsBlog.ModifiedBy = _newsBlog.UserId;
                    _newsBlog.ModifiedDate = System.DateTime.Now.ToString();
                    _newsBlogsRepo.Update(_newsBlog);
                    return Request.CreateResponse(HttpStatusCode.Accepted, "Success");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Not Found");
                }
            }
            catch (System.Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error");
            }
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

