using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class EmailNotificationsController : ApiController
    {
        IEmailNotificationsRepository _emailNotificationsRepo = RepositoryFactory.Create<IEmailNotificationsRepository>(ContextTypes.EntityFramework);

        [Route("api/EmailNotifications/GetAll/{userID}/{Type}")]
        [HttpGet]
        [AllowAnonymous]

        // GET: api/Doctor
        public HttpResponseMessage GetAll([FromUri] EmailNotifications emailNotifications, string userID, string Type)
        {
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var users = userStore.Users.ToList();
            if (Type == "Inbox")
            {
                var result = _emailNotificationsRepo.Find(x => x.To == userID && ((x.DeletedBy.IndexOf(userID) == -1) || x.DeletedBy == null)).OrderByDescending(x => x.Id).Take(10).ToList();
                var data = (from r in result
                            join u in users on r.From equals u.Id
                            select new
                            {
                                Id = r.Id,
                                To = r.To,
                                From = r.From,
                                FromName = u.FirstName + ' ' + u.LastName,
                                Description = r.Description,
                                IsDeleted = r.IsDeleted,
                                Subject = r.Subject,
                                Attachments = r.Attachments,
                                DeletedBy = r.DeletedBy,
                                CreatedDate = r.CreatedDate,
                                CreatedTime = r.CreatedTime
                            }).ToList();

                return Request.CreateResponse(HttpStatusCode.Accepted, data);
            }
            else if (Type == "Sent")
            {
                var result = _emailNotificationsRepo.Find(x => x.From == userID && ((x.DeletedBy.IndexOf(userID) == -1) || x.DeletedBy == null)).OrderByDescending(x => x.Id).Take(10).ToList();
                var data = (from r in result
                            join u in users on r.To equals u.Id
                            select new
                            {
                                Id = r.Id,
                                To = r.To,
                                From = r.From,
                                ToName = u.FirstName + ' ' + u.LastName,
                                Description = r.Description,
                                IsDeleted = r.IsDeleted,
                                Subject = r.Subject,
                                Attachments = r.Attachments,
                                DeletedBy = r.DeletedBy,
                                CreatedDate = r.CreatedDate,
                                CreatedTime = r.CreatedTime
                            }).ToList();
                return Request.CreateResponse(HttpStatusCode.Accepted, data);
            }
            else
            {
                var result = _emailNotificationsRepo.Find(x => (x.From == userID || x.To == userID) && (x.DeletedBy.IndexOf(userID) >= 0)).OrderByDescending(x => x.Id).Take(10).ToList();
                var data = (from r in result
                            join u in users on r.From equals u.Id
                            select new
                            {
                                Id = r.Id,
                                To = r.To,
                                From = r.From,
                                FromName = u.FirstName + ' ' + u.LastName,
                                Description = r.Description,
                                IsDeleted = r.IsDeleted,
                                Subject = r.Subject,
                                Attachments = r.Attachments,
                                DeletedBy = r.DeletedBy,
                                CreatedDate = r.CreatedDate,
                                CreatedTime = r.CreatedTime
                            }).ToList();
                return Request.CreateResponse(HttpStatusCode.Accepted, data);
            }
        }

        [Route("api/EmailNotifications/SaveEmail")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage SaveEmail(EmailNotifications obj)
        {
            obj.CreatedDate = DateTime.Now.ToString("dddd, dd MMMM yyyy") + " " + DateTime.Now.ToString("hh:mm tt");
            obj.CreatedTime = DateTime.Now.ToString("hh:mm tt");
            var _prescriptionCreated = _emailNotificationsRepo.Insert(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, obj.Id);
        }

        [Route("api/EmailNotifications/DeleteEmail")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage DeleteEmail(EmailNotifications emailNotifications)
        {

            if (emailNotifications.Attachments.Split(',').Length > 1)
            {
                for (int i = 0; i < emailNotifications.Attachments.Split(',').Length; i++)
                {
                    var result = _emailNotificationsRepo.Get(Convert.ToInt32(emailNotifications.Attachments.Split(',')[i]));
                    result.IsDeleted = true;
                    if (result.DeletedBy == null || result.DeletedBy == "")
                    {
                        result.DeletedBy = emailNotifications.DeletedBy;
                    }
                    else
                    {
                        result.DeletedBy = result.DeletedBy + "," + emailNotifications.DeletedBy;
                    }
                    var msgString = _emailNotificationsRepo.Update(result);
                }
            }
            else
            {
                var result = _emailNotificationsRepo.Get(Convert.ToInt32(emailNotifications.Attachments));
                result.IsDeleted = true;
                if (result.DeletedBy == null || result.DeletedBy == "")
                {
                    result.DeletedBy = emailNotifications.DeletedBy;
                }
                else
                {
                    result.DeletedBy = result.DeletedBy + "," + emailNotifications.DeletedBy;
                }
                var msgString = _emailNotificationsRepo.Update(result);

            }
            return Request.CreateResponse(HttpStatusCode.Accepted, "Deleted");
        }


        [Route("api/EmailNotifications/GetEmailByID/{id}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetEMail(int id)
        {
            var result = _emailNotificationsRepo.Get(id);
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var users = userStore.Users.Where(x => x.Id == result.To).FirstOrDefault();
            string name = users.FirstName + ' ' + users.LastName;
            var data = new { toName = name, result.Id,result.Attachments,result.CreatedDate,result.CreatedTime,result.Description,result.From,result.Subject,result.To };

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }


        [HttpPost]
        [Route("api/EmailNotifications/UploadFiles")]
        [AllowAnonymous]
        public HttpResponseMessage UploadFiles()
        {
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;
            string clientId = httpRequest.Form["ClientId"];
            var postedFile = httpRequest.Files["Files"];
            try
            {
                if (postedFile != null)
                {
                    imageName = (clientId + DateTime.Now.ToFileTime() + "_" + (postedFile.FileName).Trim()).Trim();
                    imageName = imageName.Replace(" ", string.Empty);
                    var filePath = HttpContext.Current.Server.MapPath("~/EmailNotifications/" + imageName);
                    postedFile.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            return Request.CreateResponse(HttpStatusCode.Accepted, imageName);
        }


        //[HttpGet]
        //[Route("api/EmailNotifications/Download")]
        //[AllowAnonymous]

        //[HttpGet]
        //public HttpResponseMessage DownloadFile()
        //{
        //	//string fileName = "NCM-974-073-513-807-400132294379546730214_doctor2.jpg";
        //	//string filePath = HttpContext.Current.Server.MapPath("~/EmailNotifications/" + fileName);
        //	//return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        //	// DownloadFile(Uri address, string fileName);
        //}


        //[HttpGet]
        //public async Task<IHttpActionResult> DownloadFile()
        //{
        //	var path = @"D:\NoorCare\NoorCare.Api\WebAPI\EmailNotifications\NCM-974-073-513-807-400132294379546730214_doctor2.jpg";
        //	var memory = new MemoryStream();
        //	using (var stream = new FileStream(path, FileMode.Open))
        //	{
        //		await stream.CopyToAsync(memory);

        //	}
        //	memory.Position = 0;
        //	var ext = Path.GetExtension(path).ToLowerInvariant();
        //	return File(memory, GetMineTypes()[ext], Path.GetFileName(path));

        //}

        //[HttpGet]
        //[Route("api/EmailNotifications/Download")]
        //[AllowAnonymous]
        //public HttpResponseMessage Test()
        //{
        //	string fileName = "NCM-974-073-513-807-400132294379546730214_doctor2.jpg";
        //	var path = System.Web.HttpContext.Current.Server.MapPath("~/EmailNotifications/" + fileName); ;
        //	HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //	var stream = new FileStream(path, FileMode.Open);
        //	result.Content = new StreamContent(stream);
        //	//result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //	result.Content.Headers.ContentDisposition.FileName = Path.GetFileName(path);
        //	result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //	result.Content.Headers.ContentLength = stream.Length;
        //	return result;
        //}
        private Dictionary<string, string> GetFileTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt" , "text/plain" },
                {".pdf", "application/pdf" },
                { ".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word" },
                {".xls", "application/vnd.ms-excel" },
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                {".png", "image/png" },
                {".jpg", "image/jpeg" },
                {".jped", "image/jped" },
                {".gif", "image/gif" },
                {".csv", "image/csv" }
            };
        }

        [HttpGet]//http get as it return file 
        [Route("api/EmailNotifications/Download")]
        [AllowAnonymous]
        public HttpResponseMessage GetTestFile([FromUri] EmailNotifications emailNotifications)
        {
            //below code locate physical file on server 
            //	string fileName = "NCH-974-073-022-459-800132294371051615057_ajay_FebrauryWork15th.xlsx";
            var localFilePath = System.Web.HttpContext.Current.Server.MapPath("~/EmailNotifications/" + emailNotifications.Attachments);
            HttpResponseMessage response = null;
            if (!File.Exists(localFilePath))
            {
                //if file not found than return response as resource not present 
                response = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                //if file present than read file 
                var fStream = new FileStream(localFilePath, FileMode.Open, FileAccess.Read);
                //compose response and include file as content in it
                response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StreamContent(fStream)
                };
                //set content header of reponse as file attached in reponse
                response.Content.Headers.ContentDisposition =
                new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Path.GetFileName(fStream.Name)
                };
                //set the content header content type as application/octet-stream as it      
                //returning file as reponse 
                response.Content.Headers.ContentType = new
                              MediaTypeHeaderValue("application/octet-stream");
            }
            return response;
        }

    }
}
