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
        IEnquiryRepository _enquiryRepository = RepositoryFactory.Create<IEnquiryRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _hospitaldetailsRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);

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
            string createdBy = httpRequest.Form["createdBy"];
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
                        CreatedBy = createdBy,
                        DateEntered = DateTime.Now
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
            HospitalDetails limitCount = _hospitaldetailsRepo.Find(x => x.HospitalId == facilityNoorCareNumber).FirstOrDefault();
            int imageCount = _facelityImagesRepo.Find(x => x.FacilityNoorCareNumber == facilityNoorCareNumber && x.FacilityImageType == facilityImageType).ToList().Count;
            string message = string.Empty;

            if (facilityImageType.ToLower() == "banner")
            {
                var bannerlimitCount = limitCount.LimitBannerCount == null ? 0 : limitCount.LimitBannerCount;
                int diffLimit = Convert.ToInt32(bannerlimitCount - imageCount);
                if (diffLimit <= 0)
                {
                    message = "Your have no limit for upload banner. Please delete banner and upload again";
                }
                else if (diffLimit > 0 && httpRequest.Files.Count > diffLimit)
                {
                    message = "You have only " + diffLimit + " banner limit . Please delete banner and upload again";
                }
            }
            else if (facilityImageType.ToLower() == "gallery")
            {
                var gallerylimitCount = limitCount.LimitGallaryCount == null ? 0 : limitCount.LimitGallaryCount;
                int diffLimit = Convert.ToInt32(gallerylimitCount - imageCount);
                if (diffLimit <= 0)
                {
                    message = "Your have no limit for upload gallery. Please delete gallery and upload again";
                }
                else if (diffLimit > 0 && httpRequest.Files.Count > diffLimit)
                {
                    message = "You have only " + diffLimit + " gallery limit . Please delete gallery and upload again";
                }
            }
            else if (facilityImageType.ToLower() == "vedio")
            {
                var vediolimitCount = limitCount.LimitVedioCount == null ? 0 : limitCount.LimitVedioCount;
                int diffLimit = Convert.ToInt32(vediolimitCount - imageCount);
                if (diffLimit <= 0)
                {
                    message = "Your have no limit for upload vedio. Please delete vedio and upload again";
                }
                else if (diffLimit > 0 && httpRequest.Files.Count > diffLimit)
                {
                    message = "You have only " + diffLimit + " vedio limit . Please delete vedio and upload again";
                }
            }

            if (message==string.Empty)
            {
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
                            ExpiryDate = DateTime.Now,
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
                    message = "success";
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }
            }
            return Ok(message);
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
        [Route("api/hospitaldetail/{facilitynoorcarenumber}")]
        public HospitalDetails Getfacilityimagescount(string facilitynoorcarenumber)
        {
            HospitalDetails hospital = _hospitaldetailsRepo.Find(x => x.HospitalId == facilitynoorcarenumber).FirstOrDefault();
            return hospital;
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
            var desiesTypeResultList = new List<DesiesTypeResult>();
            var disease = _diseaseDetailRepo.GetAll().OrderBy(x => x.DiseaseType).ToList();
            string host = ConfigurationManager.AppSettings.Get("ImageBaseUrl");//HttpContext.Current.Request.Url.Host;
            //var result = _quickUploadRepo.GetAll();
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
                            // Hospital Name, ID,Mobile,Email,Website,Address,UploadedBy                           
                            var fileObj = new FileName();
                            if (file.HospitalId != "Self")
                            {
                                if (file.HospitalId.Length < 22)
                                {
                                    try
                                    {
                                        var _enqObj = _enquiryRepository.GetAll().Where(y => y.Id == Convert.ToInt32(file.HospitalId)).FirstOrDefault();
                                        fileObj.HospitalId = "NA";
                                        fileObj.HospitalName = _enqObj.Name;
                                        fileObj.HospitalMobile = _enqObj.ContactNo;
                                        fileObj.HospitalEmail = _enqObj.EmailId;
                                        fileObj.HospitalWebsite = _enqObj.Website;
                                        fileObj.HospitalAddress = _enqObj.Address;
                                    }
                                    catch (Exception ex) { }

                                }
                                else if (file.HospitalId.Length >= 22)
                                {
                                    var _enqObj = _hospitaldetailsRepo.GetAll().Where(y => y.HospitalId == file.HospitalId).FirstOrDefault();
                                    fileObj.HospitalId = _enqObj.HospitalId;
                                    fileObj.HospitalName = _enqObj.HospitalName;
                                    fileObj.HospitalMobile = _enqObj.Mobile.ToString();
                                    fileObj.HospitalEmail = _enqObj.Email;
                                    fileObj.HospitalWebsite = _enqObj.Website;
                                    fileObj.HospitalAddress = _enqObj.Address;
                                }
                            }
                            var fileName = new FileName();
                            fileName.DocName = file.FilePath;
                            fileName.HospitalId = file.HospitalId;
                            fileName.Id = file.Id;
                            fileName.UploadedBy = file.CreatedBy;
                            //baseURL/ClientDocument/ClientId/DesiseType/Year/Month/Files.jpg
                            fileName.DocUrl = constant.imgUrl + "ClientDocument/" + clientId + "/" + desiesTypeResult.DiseaseType + "/" + it.AddedYear
                            + "/" + mo.AddedMonth + "/" + file.FilePath;
                            if (fileObj != null)
                            {
                                fileName.HospitalId = fileObj.HospitalId;
                                fileName.HospitalName = fileObj.HospitalName;
                                fileName.HospitalMobile = fileObj.HospitalMobile;
                                fileName.HospitalAddress = fileObj.HospitalAddress;
                                fileName.HospitalEmail = fileObj.HospitalEmail;
                                fileName.HospitalWebsite = fileObj.HospitalWebsite;
                            }
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
            string host = constant.imgUrl;// ConfigurationManager.AppSettings.Get("ImageBaseUrl");//HttpContext.Current.Request.Url.Host;

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
                            //var fileName = new FileName();
                            //fileName.DocName = file.FilePath;
                            //fileName.HospitalId = file.HospitalId;
                            //fileName.Id = file.Id;
                            //fileName.UploadedBy = file.CreatedBy;
                            ////baseURL/ClientDocument/ClientId/DesiseType/Year/Month/Files.jpg
                            //fileName.DocUrl = host + "ClientDocument/" + clientId + "/" + desiesTypeResult.DiseaseType + "/" + it.AddedYear
                            //+ "/" + mo.AddedMonth + "/" + file.FilePath;
                            //monthList.FileList.Add(fileName);
                            var fileObj = new FileName();
                            if (file.HospitalId != "Self")
                            {
                                if (file.HospitalId.Length < 22)
                                {
                                    try
                                    {
                                        var _enqObj = _enquiryRepository.GetAll().Where(y => y.Id == Convert.ToInt32(file.HospitalId)).FirstOrDefault();
                                        fileObj.HospitalId = "NA";
                                        fileObj.HospitalName = _enqObj.Name;
                                        fileObj.HospitalMobile = _enqObj.ContactNo;
                                        fileObj.HospitalEmail = _enqObj.EmailId;
                                        fileObj.HospitalWebsite = _enqObj.Website;
                                        fileObj.HospitalAddress = _enqObj.Address;
                                    }
                                    catch (Exception ex) { }

                                }
                                else if (file.HospitalId.Length >= 22)
                                {
                                    var _enqObj = _hospitaldetailsRepo.GetAll().Where(y => y.HospitalId == file.HospitalId).FirstOrDefault();
                                    fileObj.HospitalId = _enqObj.HospitalId;
                                    fileObj.HospitalName = _enqObj.HospitalName;
                                    fileObj.HospitalMobile = _enqObj.Mobile.ToString();
                                    fileObj.HospitalEmail = _enqObj.Email;
                                    fileObj.HospitalWebsite = _enqObj.Website;
                                    fileObj.HospitalAddress = _enqObj.Address;
                                }
                            }
                            var fileName = new FileName();
                            fileName.DocName = file.FilePath;
                            fileName.HospitalId = file.HospitalId;
                            fileName.Id = file.Id;
                            fileName.UploadedBy = file.CreatedBy;
                            //baseURL/ClientDocument/ClientId/DesiseType/Year/Month/Files.jpg
                            fileName.DocUrl = constant.imgUrl + "ClientDocument/" + clientId + "/" + desiesTypeResult.DiseaseType + "/" + it.AddedYear
                            + "/" + mo.AddedMonth + "/" + file.FilePath;
                            if (fileObj != null)
                            {
                                fileName.HospitalId = fileObj.HospitalId;
                                fileName.HospitalName = fileObj.HospitalName;
                                fileName.HospitalMobile = fileObj.HospitalMobile;
                                fileName.HospitalAddress = fileObj.HospitalAddress;
                                fileName.HospitalEmail = fileObj.HospitalEmail;
                                fileName.HospitalWebsite = fileObj.HospitalWebsite;
                            }
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