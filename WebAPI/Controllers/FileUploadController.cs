using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class FileUploadController : ApiController
    {
        IQuickUploadRepository _quickUploadRepo = RepositoryFactory.Create<IQuickUploadRepository>(ContextTypes.EntityFramework);

        [HttpPost]
        [Route("api/document/{clientId}/{diseaseId}")]
        public HttpResponseMessage Post(string clientId, int diseaseId)
        {
            HttpResponseMessage result = null;
            createDocPath(clientId, diseaseId);
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    docfiles.Add(filePath);
                }
                result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            return result;
        }

        private void createDocPath(string clientId, int desiesId)
        {
            string subPath = $"Documents/{clientId}/{desiesId}";
            bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
            if (!exists)
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
        }

        [HttpPost]
        [Route("api/document/uploadreport")]
        [AllowAnonymous]
        public HttpResponseMessage UploadReport(QuickUpload obj)
        {
            obj.AddedMonth = DateTime.Now.Month.ToString();
            obj.AddedYear = DateTime.Now.Year.ToString();
                        
            var _quickUploadCreated = _quickUploadRepo.Insert(obj);

            return Request.CreateResponse(HttpStatusCode.Accepted, obj.Id);
        }

        [HttpPost]
        [Route("api/document/uploadreportfile")]
        [AllowAnonymous]
        public IHttpActionResult UploadReportFile()
        {
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;
            string quickUploadId = httpRequest.Form["Id"];
            string clientId = httpRequest.Form["ClientId"];
            string desiesType = httpRequest.Form["DesiesType"];
            var postedFile = httpRequest.Files["Image"];
            string PostedFileName = string.Empty;
            string PostedFileExt = string.Empty;
            try
            {
                if (postedFile != null)
                {
                    FileInfo fi = new FileInfo(postedFile.FileName);
                    if (fi != null)
                    {
                        PostedFileName = fi.Name;
                        PostedFileExt = fi.Extension;
                    }

                    imageName = quickUploadId + PostedFileExt;

                    //File Save Path --disease type / year / month / day
                    string year = DateTime.Now.Year.ToString();
                    string month = DateTime.Now.Month.ToString();
                    //string day = DateTime.Now.Day.ToString();
                    //string time = DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString();// + DateTime.Now.Second.ToString();

                    //var filePath = HttpContext.Current.Server.MapPath("~/ClientDocument/" + desiesType + "/" + clientId + "/" + year + "/" + month + "/" + day);
                    var filePath = HttpContext.Current.Server.MapPath("~/ClientDocument/" + clientId + "/" + desiesType + "/" + year + "/"+ month);
                    // bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/ClientDocument/" + desiesType + "/" + year + "/" + month + "/" + day));
                    //if (exists)
                    //{
                    //    File.Delete(filePath);
                    //}
                    Directory.CreateDirectory(filePath);
                    filePath = filePath + "/" + imageName;
                 
                    postedFile.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
            }
            return Ok(quickUploadId);
        }

        #region GetDisease

        [Route("api/GetUploadedDiseaseList/{clientId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetUploadedDiseaseList(string clientId)
        {
            var result = _quickUploadRepo.Find(x => x.ClientId == clientId);

            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/GetUploadedYearList/{clientId}/{disease}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetUploadedYearList(string clientId, string disease)
        {
            var result = _quickUploadRepo.Find(x => x.ClientId == clientId && x.DesiesType==disease);

            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/GetUploadedMonthList/{clientId}/{disease}/{year}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetUploadedMonthList(string clientId, string disease, string year)
        {
            var result = _quickUploadRepo.Find(x => x.ClientId == clientId && x.DesiesType == disease && x.AddedYear== year);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/GetUploadedFilesList/{clientId}/{disease}/{year}/{month}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetUploadedFilesList(string clientId, string disease, string year, string month)
        {
            var result = _quickUploadRepo.Find(x => x.ClientId == clientId && x.DesiesType == disease && x.AddedYear == year && x.AddedMonth==month);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }


        #endregion

    }
}