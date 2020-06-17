using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.OData;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class InsuranceinfoController : ApiController
    {
        IInsuranceInformationRepository _insuranceInformationRepository =
            RepositoryFactory.Create<IInsuranceInformationRepository>(ContextTypes.EntityFramework);

        [HttpGet]
        [Route("api/get/insuranceinfo/{ClientID}")]
        public IHttpActionResult getInsurancenformationt(string ClientID)
        {
            string type = "";
            string ID = ClientID.Split('-')[0];
            if (ID == "NCM" || ID == "NCF")
            {
                type = "Patient";
            }
            else
            {
                type = "Hospital";
            }
            InsuranceInformation _insuranceContact = _insuranceInformationRepository.Find(x => x.ClientId == ClientID && x.Type == type).FirstOrDefault();
            return Ok(_insuranceContact);
        }

        [HttpGet]
        [Route("api/getall/insuranceinfo/{ClientId}")]
        [AllowAnonymous]
        public IHttpActionResult getallInsurancenformationt(string ClientId)
        {
            string type = "";
            string ID = ClientId.Split('-')[0];
            if (ID == "NCM" || ID == "NCF")
            {
                type = "Patient";
            }
            else
            {
                type = "Hospital";
            }
            List<InsuranceInformation> _insuranceContact = _insuranceInformationRepository.Find(x => x.ClientId == ClientId &&x.Type==type).ToList();
            return Ok(_insuranceContact);
        }
        [HttpGet]
        [Route("api/get/insurancedetail/{insuranceId}")]
        public IHttpActionResult getinsurancedetail(int insuranceId)
        {
            InsuranceInformation _insuranceContact = _insuranceInformationRepository.Find(x => x.Id == insuranceId).FirstOrDefault();
            return Ok(_insuranceContact);
        }

        [HttpPost]
        [Route("api/add/insuranceinfo/{ClientId}")]
        [AllowAnonymous]
        public IHttpActionResult AddInsurancenformationt(string ClientId, InsuranceInformation insuranceInformation)
        {
            string type = "";
            string ID = ClientId.Split('-')[0];
            if (ID == "NCM" || ID == "NCF")
            {
                type = "Patient";
            }
            else
            {
                type = "Hospital";
            }
            InsuranceInformation _insuranceInformation = new InsuranceInformation
            {
                ClientId = ClientId,
                CompanyName = insuranceInformation.CompanyName,
                InsuraceNo = insuranceInformation.InsuraceNo,
                ExpiryDate = insuranceInformation.ExpiryDate,
                IsActive = true,
                Type= type


            };
            return Ok(_insuranceInformationRepository.Insert(_insuranceInformation));
        }

        [HttpPost]
        [Route("api/update/insuranceinfo")]
        public IHttpActionResult UpdateInsurancenformationt(InsuranceInformation insuranceInformation)
        {
            InsuranceInformation _insuranceInformation = _insuranceInformationRepository.Find(x => x.Id == insuranceInformation.Id).FirstOrDefault();
            _insuranceInformation.CompanyName = insuranceInformation.CompanyName;
            _insuranceInformation.InsuraceNo = insuranceInformation.InsuraceNo;
            _insuranceInformation.ExpiryDate = insuranceInformation.ExpiryDate;
            return Ok(_insuranceInformationRepository.Update(_insuranceInformation));
        }

        [HttpPost]
        [Route("api/delete/insuranceinfo")]
        public IHttpActionResult DeleteInsurancenformationt(InsuranceInformation insuranceInformation)
        {
            InsuranceInformation _insuranceInformation = _insuranceInformationRepository.Find(x => x.Id == insuranceInformation.Id).FirstOrDefault();
            _insuranceInformation.IsActive = insuranceInformation.IsActive;
            return Ok(_insuranceInformationRepository.Update(_insuranceInformation));
        }
    }
}