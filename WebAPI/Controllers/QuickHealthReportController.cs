using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            QuickHeathDetails _quickHeathDetails = _quickHealthRepository.Find(x => x.ClientId == ClientId).FirstOrDefault();
            return Ok(_quickHeathDetails);
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
            return Ok(_quickHealthRepository.Insert(quickHeathDetails));
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
