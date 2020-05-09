using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        IFacilityImagesRepository _facelityImagesRepo = RepositoryFactory.Create<IFacilityImagesRepository>(ContextTypes.EntityFramework);
        IQuickUploadAssignRepository _QuickUploadAssignRepo = RepositoryFactory.Create<IQuickUploadAssignRepository>(ContextTypes.EntityFramework);

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
            string hospitalId = httpRequest.Form["HospitalId"];
            string clientId = httpRequest.Form["ClientId"];
            string diseaseType = httpRequest.Form["diseaseType"];
            var postedFile = httpRequest.Files["Image"];
            string PostedFileName = string.Empty;
            string PostedFileExt = string.Empty;
            ////File Information Save in Database
            QuickUpload quickHeathDetails = new QuickUpload
            {
                ClientId = clientId,
                HospitalId = hospitalId,
                DesiesType = Convert.ToInt32(diseaseType),
                AddedYear = DateTime.Now.Year,
                AddedMonth = DateTime.Now.Month,
                FilePath = postedFile.FileName,
            };
            var objId = _quickUploadRepo.Insert(quickHeathDetails);
            try
            {
                if (postedFile != null)
                {
                    FileInfo fi = new FileInfo(postedFile.FileName.Replace(" ", "_"));
                    if (fi != null)
                    {
                        PostedFileName = fi.Name;
                        PostedFileExt = fi.Extension;
                    }

                    imageName = objId + PostedFileExt;

                    string year = DateTime.Now.Year.ToString();
                    string month = DateTime.Now.Month.ToString();
                    var filePath = HttpContext.Current.Server.MapPath("~/ClientDocument/" + clientId + "/" + diseaseType + "/" + year + "/" + month);
                    Directory.CreateDirectory(filePath);
                    filePath = filePath + "/" + imageName;

                    postedFile.SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
            }
            return Ok(objId);
        }

        [HttpPost]
        [Route("api/document/uploadreportfile1")]
        [AllowAnonymous]
        public IHttpActionResult UploadReportFile1()
        {
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;
            string hospitalId = httpRequest.Form["HospitalId"];
            string clientId = httpRequest.Form["ClientId"];
            string diseaseType = httpRequest.Form["diseaseType"];
            //var postedFile = httpRequest.Files["Image"];
            string PostedFileName = string.Empty;
            string PostedFileExt = string.Empty;
            // string fileName = "";
            //for (int i = 0; i < httpRequest.Files.Count; i++)
            //{
            //    fileName = fileName + "," + httpRequest.Files[i].FileName;
            //}
            ////File Information Save in Database
            int objId = 0;
            try
            {

                string year = DateTime.Now.Year.ToString();
                string month = DateTime.Now.Month.ToString();
                var directoryPath = Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/ClientDocument/" + clientId + "/" + diseaseType + "/" + year + "/" + month));
                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    QuickUpload quickHeathDetails = new QuickUpload
                    {
                        ClientId = clientId,
                        HospitalId = hospitalId,
                        DesiesType = Convert.ToInt32(diseaseType),
                        AddedYear = DateTime.Now.Year,
                        AddedMonth = DateTime.Now.Month,
                        FilePath = httpRequest.Files[i].FileName,
                    };
                    objId = _quickUploadRepo.Insert(quickHeathDetails);
                    FileInfo fi = new FileInfo(httpRequest.Files[i].FileName.Replace(" ", "_"));
                    if (fi != null)
                    {
                        PostedFileName = fi.Name;
                        PostedFileExt = fi.Extension;
                    }
                    imageName = objId + i + PostedFileExt;
                    var filePath = HttpContext.Current.Server.MapPath("~/ClientDocument/" + clientId + "/" + diseaseType + "/" + year + "/" + month);
                    filePath = filePath + "/" + httpRequest.Files[i].FileName;
                    httpRequest.Files[i].SaveAs(filePath);

                }
            }
            catch (Exception ex)
            {
            }
            return Ok(objId);
        }

        // Get Facility Image
        [HttpGet]
        [Route("api/facilityImage/GetFacilityImage/{FacilityNoorCare}")]
        [AllowAnonymous]
        public HttpResponseMessage GetFacilityImage(string FacilityNoorCare)
        {
            try
            {
                var FacilityIMG = _facelityImagesRepo.Find(x => x.FacilityNoorCareNumber == FacilityNoorCare).ToList();

                return Request.CreateResponse(HttpStatusCode.Accepted, FacilityIMG);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.Accepted, "Error");

            }
        }

        [HttpPost]
        [Route("api/facilityImages/uploadfacilityimges")]
        [AllowAnonymous]
        public IHttpActionResult uploadfacilityimges()
        {
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;
            string facilityImageType = httpRequest.Form["FacilityImageType"];
            string facilityNoorCareNumber = httpRequest.Form["FacilityNoorCareNumber"];
            //var postedFile = httpRequest.Files["Image"];

            var imageCount = _facelityImagesRepo.Find(x => x.FacilityNoorCareNumber == facilityNoorCareNumber).ToList().Count;
            string PostedFileName = string.Empty;
            string PostedFileExt = string.Empty;
            int objId = 0;
            try
            {
                string year = DateTime.Now.Year.ToString();
                string month = DateTime.Now.Month.ToString();
                string directoryPath = string.Empty;
                if (!Directory.Exists(HttpContext.Current.Server.MapPath("~/FacilityImages/" + facilityNoorCareNumber + "/" + facilityImageType)))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~/FacilityImages/" + facilityNoorCareNumber + "/" + facilityImageType));
                }

                for (int i = 0; i < httpRequest.Files.Count; i++)
                {
                    var filePath = HttpContext.Current.Server.MapPath("~/FacilityImages/" + facilityNoorCareNumber + "/" + facilityImageType);
                    filePath = filePath + "/" + httpRequest.Files[i].FileName;
                    FacilityImages facilityImages = new FacilityImages
                    {
                        FacilityImageType = facilityImageType,
                        FacilityNoorCareNumber = facilityNoorCareNumber,
                        ExpiryDate = DateTime.Now.ToString(),
                        FileName = httpRequest.Files[i].FileName,
                        FilePath = "FacilityImages/" + facilityNoorCareNumber + "/" + facilityImageType + "/" + httpRequest.Files[i].FileName,
                        DateEntered = DateTime.Now,
                    };
                    objId = _facelityImagesRepo.Insert(facilityImages);
                    FileInfo fi = new FileInfo(httpRequest.Files[i].FileName.Replace(" ", "_"));
                    if (fi != null)
                    {
                        PostedFileName = fi.Name;
                        PostedFileExt = fi.Extension;
                    }
                    imageName = objId + i + PostedFileExt;
                    httpRequest.Files[i].SaveAs(filePath);
                }
            }
            catch (Exception ex)
            {
            }
            return Ok(objId);
        }

        [HttpPost]
        [Route("api/document/uploadreport")]
        [AllowAnonymous]
        public IHttpActionResult UploadReport()
        {
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;
            string hospitalId = httpRequest.Form["HospitalId"];
            string clientId = httpRequest.Form["ClientId"];
            string diseaseType = httpRequest.Form["diseaseType"];
            var postedFile = httpRequest.Files["Image"];
            string PostedFileName = string.Empty;
            string PostedFileExt = string.Empty;
            ////File Information Save in Database
            QuickUpload quickHeathDetails = new QuickUpload
            {
                ClientId = clientId,
                //HospitalId = hospitalId,
                DesiesType = Convert.ToInt32(diseaseType),
                AddedYear = DateTime.Now.Year,
                AddedMonth = DateTime.Now.Month,
                FilePath = postedFile.FileName,
            };
            var objId = _quickUploadRepo.Insert(quickHeathDetails);
            try
            {
                if (postedFile != null)
                {
                    FileInfo fi = new FileInfo(postedFile.FileName.Replace(" ", "_"));
                    if (fi != null)
                    {
                        PostedFileName = fi.Name;
                        PostedFileExt = fi.Extension;
                    }

                    imageName = objId + PostedFileExt;

                    string year = DateTime.Now.Year.ToString();
                    string month = DateTime.Now.Month.ToString();
                    var filePath = HttpContext.Current.Server.MapPath("~/ClientDocument/" + clientId + "/" + diseaseType + "/" + year + "/" + month);
                    Directory.CreateDirectory(filePath);
                    filePath = filePath + "/" + imageName;

                    postedFile.SaveAs(filePath);


                }
            }
            catch (Exception ex)
            {
            }
            return Ok(objId);
        }

        [HttpGet]
        [Route("api/facilityimagescount/{type}/{facilitynoorcarenumber}")]
        public int Gefacilityimagescount(string facilitynoorcarenumber, string type)
        {
            var imageCount = _facelityImagesRepo.Find(x => x.FacilityNoorCareNumber == facilitynoorcarenumber && x.FacilityImageType == type).ToList().Count;
            return imageCount;
        }

        #region Quick Upload Assign

        [HttpPost]
        [Route("api/document/assign")]
        [AllowAnonymous]
        public IHttpActionResult AssignDocs(QuickUploadAssign[] data)
        {
            try
            {
                int id = 0;
                if (data != null)
                {
                    foreach (QuickUploadAssign element in data)
                    {
                        string assignId = element.AssignId;
                        int quickUploadId = element.QuickUploadId;
                        int count = _QuickUploadAssignRepo.Find(x => x.AssignId == assignId && x.QuickUploadId == quickUploadId).Count;
                        if (count <= 0)
                        {
                            id = _QuickUploadAssignRepo.Insert(element);
                        }

                    }
                }
                return Ok(id);

            }
            catch (Exception ex)
            {
                return Ok("Dharmendra");
                //return InternalServerError(ex);
            }
            //return Ok(mailBoxid);
        }

        #endregion

        #region GetDisease

        [Route("api/GetUploadedDocInfo/{clientId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetUploadedDocInfo(string clientId)
        {
           // IDoctorRepository _doctorRepo = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);

            var desiesTypeResultList = new List<DesiesTypeResult>();
            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
            string host = ConfigurationManager.AppSettings.Get("ImageBaseUrl");//HttpContext.Current.Request.Url.Host;
            var result = _quickUploadRepo.Find(x => x.ClientId == clientId);

            //int count = _doctorRepo.Find(doc => doc.DoctorId == clientId).Count;
            //if (count > 0)
            //{
            //    var quickUp = _quickUploadRepo.GetAll().ToList();
            //    var quickUpAssign = _QuickUploadAssignRepo.GetAll().ToList();

            //    var appointDetail = (from a in quickUp
            //                         join t in quickUpAssign on a.Id equals t.QuickUploadId
            //                         where t.AssignId == clientId && t.IsActive == true
            //                         select a).ToList();
            //    result = appointDetail;
            //}


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
                if (x.diseaseType != 0)
                {
                    desiesTypeResult.DiseaseType = x.diseaseType;
                    desiesTypeResult.DesiesName = disease.Where(c => c.Id == x.diseaseType).FirstOrDefault().DiseaseType;
                }
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
                            fileName.HospitalId = file.HospitalId;
                            fileName.Id = file.Id;
                            //baseURL/ClientDocument/ClientId/DesiseType/Year/Month/Files.jpg
                            fileName.DocUrl = host + "/ClientDocument/" + clientId + "/" + desiesTypeResult.DiseaseType + "/" + it.AddedYear
                            + "/" + mo.AddedMonth + "/" + file.FilePath;
                            monthList.FileList.Add(fileName);
                        }
                    }

                    desiesTypeResult.YearList.Add(yearList);
                }

                desiesTypeResultList.Add(desiesTypeResult);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, desiesTypeResultList);
        }


        [Route("api/GetUploadedDocInfo/{clientId}/{doctorId}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetUploadedDocInfo(string clientId, string doctorId)
        {
            var desiesTypeResultList = new List<DesiesTypeResult>();
            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
            string host = ConfigurationManager.AppSettings.Get("ImageBaseUrl");//HttpContext.Current.Request.Url.Host;

            var quickUp = _quickUploadRepo.GetAll().ToList();
            var quickUpAssign = _QuickUploadAssignRepo.GetAll().ToList();

            var appointDetail = (from a in quickUp
                                 join t in quickUpAssign on a.Id equals t.QuickUploadId
                                 where t.AssignId == doctorId && t.AssignBy == clientId && t.IsActive == true
                                 select a).ToList();
            var result = appointDetail;


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
                if (x.diseaseType != 0)
                {
                    desiesTypeResult.DiseaseType = x.diseaseType;
                    desiesTypeResult.DesiesName = disease.Where(c => c.Id == x.diseaseType).FirstOrDefault().DiseaseType;
                }
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
                            fileName.HospitalId = file.HospitalId;
                            fileName.Id = file.Id;
                            //baseURL/ClientDocument/ClientId/DesiseType/Year/Month/Files.jpg
                            fileName.DocUrl = host + "/ClientDocument/" + clientId + "/" + desiesTypeResult.DiseaseType + "/" + it.AddedYear
                            + "/" + mo.AddedMonth + "/" + file.FilePath;
                            monthList.FileList.Add(fileName);
                        }
                    }

                    desiesTypeResult.YearList.Add(yearList);
                }

                desiesTypeResultList.Add(desiesTypeResult);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted, desiesTypeResultList);
        }
        #endregion



    }
}