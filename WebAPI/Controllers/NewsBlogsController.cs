using NoorCare.Repository;
using System;
using System.Data.Entity.SqlServer;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class NewsBlogsController : ApiController
    {
        INewsBlogsRepository _newsBlogsRepo = RepositoryFactory.Create<INewsBlogsRepository>(ContextTypes.EntityFramework);
        IReadLikeRepository _readLikeRepo = RepositoryFactory.Create<IReadLikeRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _hospitalDetailsRepository = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        ISecretaryRepository _secretaryRepository = RepositoryFactory.Create<ISecretaryRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _doctorRepository = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        [Route("api/NewsBlogs/getAllNewsBlogs/{Type}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAllNewsBlogs([FromUri] NewsBlogs newsBlogs, string Type)
        {

            var result = _newsBlogsRepo.Find(
                 x => x.Category == Type.ToLower()
                 && x.IsDeleted==false
                 && (newsBlogs.UserId == null || newsBlogs.UserId.Contains(x.UserId))
                 && (newsBlogs.NewsCategory == null || x.NewsCategory.ToUpper().Contains(newsBlogs.NewsCategory.ToUpper()))
                 && (newsBlogs.NewsTitle == null || x.NewsTitle.ToUpper().Contains(newsBlogs.NewsTitle.ToUpper()))).OrderByDescending(x => x.Id).Take(15).ToList();

         
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/NewsBlogs/{clientId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetNewsBlog(string clientId)
        {
            var result = _newsBlogsRepo.GetAll().Where(x => x.UserId == clientId && x.IsDeleted == false).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [Route("api/NewsBlogs/GetNewsBlog/{id}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetNewsBlog(int id)
        {
            var result = _newsBlogsRepo.Get(id);

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        [Route("api/NewsBlogs/UploadNewsImage")]
        [AllowAnonymous]
        public HttpResponseMessage AddNews()
        {
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;
            string clientId = httpRequest.Form["ClientId"];
            var postedFile = httpRequest.Files["Image"];
            try
            {
                if (postedFile != null)
                {
                    imageName = clientId + DateTime.Now.ToFileTime() + "_" + postedFile.FileName;
                    var filePath = HttpContext.Current.Server.MapPath("~/ImgNewsBlog/" + imageName);
                    postedFile.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, imageName);
        }

        [Route("api/NewsBlogs/SaveNewsBlog")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage SaveNewsBlog(NewsBlogs obj)
        {
            if (obj.UserId.Contains("NCH"))
            {
                var hospital = _hospitalDetailsRepository.Find(x => x.HospitalId == obj.UserId).FirstOrDefault();
                if (hospital!=null)
                {
                    obj.CreatedBy = hospital.HospitalName;
                }
            }
            else
            {
                var doctor = _doctorRepository.Find(x => x.DoctorId == obj.UserId).FirstOrDefault();
                if (doctor != null)
                {
                    obj.CreatedBy = doctor.FirstName +" "+ doctor.LastName;
                }
            }
            var _newsBlogCreated = _newsBlogsRepo.Insert(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, obj.Id);
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
                    _newsBlog.NewsTitle = obj.NewsTitle;
                    _newsBlog.ImageURL = obj.ImageURL;
                    _newsBlog.ModifiedBy = obj.UserId;
                    _newsBlog.ModifiedDate = System.DateTime.Now;
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
        [HttpGet]
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
                    _newsBlog.ModifiedDate = System.DateTime.Now;
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

        [Route("api/NewsBlogs/Like/{id}")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Update(int id)
        {
            var result = _newsBlogsRepo.Get(id);
            result.NoOfLikes = (result.NoOfLikes) + 1;
            var msgString = _newsBlogsRepo.Update(result);
            return Request.CreateResponse(HttpStatusCode.Accepted, msgString);
        }

        [Route("api/NewsBlogs/Read/{id}")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage UpdateRead(int id)
        {
            var result = _newsBlogsRepo.Get(id);
            result.NoOfRead = (result.NoOfRead) + 1;
            var msgString = _newsBlogsRepo.Update(result);
            return Request.CreateResponse(HttpStatusCode.Accepted, msgString);
        }


    }
}

