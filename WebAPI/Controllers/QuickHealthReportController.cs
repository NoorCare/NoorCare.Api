﻿using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class QuickHealthReportController : ApiController
    {
        IQuickHealthRepository _quickHealthRepository =
            RepositoryFactory.Create<IQuickHealthRepository>(ContextTypes.EntityFramework);

        IHospitalDetailsRepository _hospitalDetailsRepository =
            RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);

        IQuickUploadRepository _quickUploadRepository =
            RepositoryFactory.Create<IQuickUploadRepository>(ContextTypes.EntityFramework);

        [HttpGet]
        [Route("api/user/get/quickHealth/{ClientId}")]
        [AllowAnonymous]
        public IHttpActionResult getQuickHealth(string ClientId)
        {
            QuickHeathDetails _quickHeathDetails = _quickHealthRepository.Find(x => x.ClientId == ClientId).LastOrDefault();
            return Ok(_quickHeathDetails);
        }

        [HttpPost]
        [Route("api/user/add/quickHealth")]
        [AllowAnonymous]
        public IHttpActionResult UploadReportFile()
        {
            string imageName = null;
            var httpRequest = HttpContext.Current.Request;
            string clientId = httpRequest.Form["ClientId"];
            string diseaseType = httpRequest.Form["diseaseType"];
             var postedFile = httpRequest.Files["Image"];
            string PostedFileName = string.Empty;
            string PostedFileExt = string.Empty;
            ////File Information Save in Database
            QuickHeathDetails quickHeathDetails = new QuickHeathDetails
            {
                ClientId = httpRequest.Form["ClientId"],
                Pressure = httpRequest.Form["Pressure"],
                Heartbeats = httpRequest.Form["Heartbeats"],
                Temprature = httpRequest.Form["Temprature"],
                Sugar = httpRequest.Form["Sugar"],
                Length = httpRequest.Form["Length"],
                Weight = httpRequest.Form["Weight"],
                Cholesterol = httpRequest.Form["Cholesterol"],
                Other = httpRequest.Form["Other"],
            };
            var objId = _quickHealthRepository.Insert(quickHeathDetails);
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
                    var filePath = HttpContext.Current.Server.MapPath("~/ClientDocument/" + httpRequest.Form["ClientId"] + "/" + diseaseType + "/" + year + "/" + month);
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
        [Route("api/user/add/quickHealth/{ClientId}")]
        [AllowAnonymous]
        public IHttpActionResult AddQuickHeathDetails(string ClientId, QuickHeathDetails _quickHeathDetails)
        {

            QuickHeathDetails quickHeathDetails = new QuickHeathDetails
            {
                ClientId = ClientId,
                HospitalId = _quickHeathDetails.HospitalId,
                Pressure = _quickHeathDetails.Pressure,
                Heartbeats = _quickHeathDetails.Heartbeats,
                Temprature = _quickHeathDetails.Temprature,
                Sugar = _quickHeathDetails.Sugar,
                Length = _quickHeathDetails.Length,
                Weight = _quickHeathDetails.Weight,
                Cholesterol = _quickHeathDetails.Cholesterol,
                Other = _quickHeathDetails.Other,
            };
           // _quickHealthRepository.Insert(quickHeathDetails);
            return Ok(_quickHealthRepository.Insert(quickHeathDetails));
        }

        private void createDocPath(string clientId, int desiesId)
        {
            string subPath = $"Documents/{clientId}/{desiesId}";
            bool exists = System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(subPath));
            if (!exists)
                System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(subPath));
        }

        [HttpPost]
        [Route("api/user/add/quickUpload/{ClientId}")]
        [AllowAnonymous]
        public IHttpActionResult AddQuickUpload(string ClientId, QuickUpload _quickUpload)
        {
            QuickUpload quickHeathDetails = new QuickUpload
            {
                ClientId = ClientId,
                HospitalId = _quickUpload.HospitalId,
                DesiesType = _quickUpload.DesiesType,
            };
            return Ok(_quickUploadRepository.Insert(quickHeathDetails));
        }

        [HttpPost]
        [Route("api/user/add/hospitalDetail")]
        [AllowAnonymous]
        public IHttpActionResult AddHospital(HospitalDetails _hospitalDetail)
        {
            HospitalDetails _HospitalDetail = _hospitalDetailsRepository.Find(
                x => x.HospitalName.ToLower() == _hospitalDetail.HospitalName.ToLower()
                || x.HospitalName.ToLower() == _hospitalDetail.HospitalName.ToLower()
                || x.Mobile == _hospitalDetail.Mobile
                || x.Email.ToLower() == _hospitalDetail.Email.ToLower()
                || x.Website.ToLower() == _hospitalDetail.Website.ToLower()
                ).FirstOrDefault();
            if (_HospitalDetail == null)
            {
                HospitalDetails hospitalDetail = new HospitalDetails
                {
                    HospitalName = _hospitalDetail.HospitalName,
                    Address = _hospitalDetail.Address,
                    Mobile = _hospitalDetail.Mobile,
                    Email = _hospitalDetail.Email,
                    Website = _hospitalDetail.Website,
                };
                return Ok(_hospitalDetailsRepository.Insert(hospitalDetail));
            }
            else
            {
                return Ok(_HospitalDetail);
            }
        }
    }
}
