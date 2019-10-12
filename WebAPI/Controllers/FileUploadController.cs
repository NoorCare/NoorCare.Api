using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class FileUploadController : ApiController
    {
        IQuickUploadRepository _quickUploadRepo = RepositoryFactory.Create<IQuickUploadRepository>(ContextTypes.EntityFramework);
        IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(ContextTypes.EntityFramework);

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
            obj.AddedMonth = DateTime.Now.Month;
            obj.AddedYear = DateTime.Now.Year;

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
            string diseaseType = httpRequest.Form["diseaseType"];
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

                    //var filePath = HttpContext.Current.Server.MapPath("~/ClientDocument/" + diseaseType + "/" + clientId + "/" + year + "/" + month + "/" + day);
                    var filePath = HttpContext.Current.Server.MapPath("~/ClientDocument/" + clientId + "/" + diseaseType + "/" + year + "/" + month);
                    // bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath("~/ClientDocument/" + diseaseType + "/" + year + "/" + month + "/" + day));
                    //if (exists)
                    //{
                    //    File.Delete(filePath);
                    //}
                    Directory.CreateDirectory(filePath);
                    filePath = filePath + "/" + imageName;

                    postedFile.SaveAs(filePath);
                    ////File Information Save in Database
                    //QuickUpload quickHeathDetails = new QuickUpload
                    //{
                    //    ClientId = clientId,
                    //    HospitalId = hospitalId,
                    //    diseaseType = diseaseType,
                    //    AddedYear= year,
                    //    AddedMonth = month,
                    //    FilePath = imageName,
                    //};
                    //_quickUploadRepo.Insert(quickHeathDetails);

                }
            }
            catch (Exception ex)
            {
            }
            return Ok(quickUploadId);
        }

        #region GetDisease

        [Route("api/GetUploadedDocInfo/{clientId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetUploadedDocInfo(string clientId)
        {
            var desiesTypeResultList = new List<DesiesTypeResult>();
            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
            string host = HttpContext.Current.Request.Url.Host;
            var result = _quickUploadRepo.Find(x => x.ClientId == clientId);
            var list = result.ToList();
            var data = list.GroupBy(item => item.DesiesType)
               .Select(group => new { diseaseType = group.Key, Items = group.ToList() })
               .ToList();

            foreach (var x in data)
            {
                var desiesTypeResult = new DesiesTypeResult();
                var listYear = x.Items.Select(xx => xx).ToList().GroupBy(p => p.AddedYear).
                    Select(group => new { AddedYear = group.Key, Items = group.ToList() })
               .ToList();
                desiesTypeResult.Years = new List<int?>();
                desiesTypeResult.DiseaseType = x.diseaseType;
                desiesTypeResult.DesiesName = disease.Where(c => c.Id == x.diseaseType).FirstOrDefault().DiseaseType;
                desiesTypeResult.YearList = new List<YearList>();
                foreach (var it in listYear)
                {

                    desiesTypeResult.Years.Add(it.AddedYear);
                    var yearList = new YearList();
                    yearList.Year = it.AddedYear;

                    var listMonth = it.Items.Select(xxp => xxp).ToList().GroupBy(ppp => ppp.AddedMonth).
                    Select(group => new { AddedMonth = group.Key, Items = group.ToList() })
                  .ToList();
                    yearList.Month = new List<int?>();
                    yearList.MonthList = new List<MonthList>();
                    foreach (var mo in listMonth)
                    {
                        yearList.Month.Add(mo.AddedMonth);
                        var monthList = new MonthList();
                        monthList.Month = mo.AddedMonth;

                        yearList.MonthList.Add(monthList);
                        //MonthList
                        // To get Files details
                        monthList.FileList = new List<FileName>();
                        foreach (var file in mo.Items)
                        {
                            var fileName = new FileName();
                            fileName.DocName = file.FilePath;
                            //baseURL/ClientDocument/ClientId/DesiseType/Year/Month/Files.jpg
                            fileName.DocUrl = host + "/ClientDocument/" + clientId + "/" + desiesTypeResult.DiseaseType + "/" + it.AddedYear
                                + "/" + mo.AddedMonth + "/" + file.FilePath;
                            monthList.FileList.Add(fileName);
                        }
                    }

                    desiesTypeResult.YearList.Add(yearList);
                }

                desiesTypeResultList.Add(desiesTypeResult);
                //desiesTypeResult.Years.AddRange(listYear);

            }

            return Request.CreateResponse(HttpStatusCode.Accepted, desiesTypeResultList);
        }

        //[Route("api/GetUploadedYearList/{clientId}/{disease}")]
        //[HttpGet]
        //[AllowAnonymous]
        //public HttpResponseMessage GetUploadedYearList(string clientId, string disease)
        //{
        //    var result = _quickUploadRepo.Find(x => x.ClientId == clientId && x.diseaseType==disease);

        //    return Request.CreateResponse(HttpStatusCode.Accepted, result);
        //}

        //[Route("api/GetUploadedMonthList/{clientId}/{disease}/{year}")]
        //[HttpGet]
        //[AllowAnonymous]
        //public HttpResponseMessage GetUploadedMonthList(string clientId, string disease, string year)
        //{
        //    var result = _quickUploadRepo.Find(x => x.ClientId == clientId && x.diseaseType == disease && x.AddedYear== year);
        //    return Request.CreateResponse(HttpStatusCode.Accepted, result);
        //}

        //[Route("api/GetUploadedFilesList/{clientId}/{disease}/{year}/{month}")]
        //[HttpGet]
        //[AllowAnonymous]
        //public HttpResponseMessage GetUploadedFilesList(string clientId, string disease, string year, string month)
        //{
        //    var result = _quickUploadRepo.Find(x => x.ClientId == clientId && x.diseaseType == disease && x.AddedYear == year && x.AddedMonth==month);
        //    return Request.CreateResponse(HttpStatusCode.Accepted, result);
        //}



        #endregion

    }
}