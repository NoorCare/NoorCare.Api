﻿using NoorCare.Repository;
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
    public class UserController : ApiController
    {
        IEmergencyContactRepository _emergencyContactRepository =
            RepositoryFactory.Create<IEmergencyContactRepository>(ContextTypes.EntityFramework);

        IMedicalInformationRepository _medicalInformationRepository =
            RepositoryFactory.Create<IMedicalInformationRepository>(ContextTypes.EntityFramework);

        IInsuranceInformationRepository _insuranceInformationRepository =
            RepositoryFactory.Create<IInsuranceInformationRepository>(ContextTypes.EntityFramework);

        /// <summary>
        /// Emergency Contact Detail
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="emergencyContact"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/user/add/emergencyContact/{ClientId}")]
        public IHttpActionResult AddEmergencyContact(string ClientId, EmergencyContact emergencyContact)
        {
            EmergencyContact _clientDetail = new EmergencyContact
            {
                clientId = ClientId,
                FirstName = emergencyContact.FirstName,
                LastName = emergencyContact.LastName,
                Gender = emergencyContact.Gender,
                Relationship = emergencyContact.Relationship,
                Email = emergencyContact.Email,
                Mobile = emergencyContact.Mobile,
                AlternateNumber = emergencyContact.AlternateNumber,
                WorkNumber = emergencyContact.WorkNumber,
                Address = emergencyContact.Address,
            };
            return Ok(_emergencyContactRepository.Insert(_clientDetail));
        }

        [HttpPost]
        [Route("api/user/update/emergencyContact/{ClientId}")]
        public IHttpActionResult UpdateEmergencyContact(string ClientId, EmergencyContact emergencyContact)
        {
            EmergencyContact eContact = _emergencyContactRepository.Find(x => x.clientId == ClientId).FirstOrDefault();

            eContact.FirstName = emergencyContact.FirstName;
            eContact.LastName = emergencyContact.LastName;
            eContact.Gender = emergencyContact.Gender;
            eContact.Relationship = emergencyContact.Relationship;
            eContact.Email = emergencyContact.Email;
            eContact.Mobile = emergencyContact.Mobile;
            eContact.AlternateNumber = emergencyContact.AlternateNumber;
            eContact.WorkNumber = emergencyContact.WorkNumber;
            eContact.Address = emergencyContact.Address;
            return Ok(_emergencyContactRepository.Update(eContact));
        }

        [HttpGet]
        [Route("api/user/get/emergencycontact/{ClientId}")]
        public IHttpActionResult getEmergencyContact(string ClientId)
        {
            EmergencyContact _emContact = _emergencyContactRepository.Find(x => x.clientId == ClientId).FirstOrDefault();
            return Ok(_emContact);
        }

        /// <summary>
        /// Medical Information
        /// </summary>
        /// <param name="ClientId"></param>
        /// <param name="emergencyContact"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/user/add/medicalinfo/{ClientId}")]
        public IHttpActionResult AddMedicalInformation(string ClientId, MedicalInformation medicalInformation)
        {
            MedicalInformation _medicalInformation = new MedicalInformation
            {
                clientId = ClientId,
                Hight = medicalInformation.Hight,
                Wight = medicalInformation.Wight,
                BloodGroup = medicalInformation.BloodGroup,
                Diseases = medicalInformation.Diseases,
                AnyAllergies = medicalInformation.AnyAllergies,
                AnyHealthCrisis = medicalInformation.AnyHealthCrisis,
                AnyRegularMedication = medicalInformation.AnyRegularMedication,
                Disability = medicalInformation.Disability,
                Smoke = medicalInformation.Smoke,
                Drink = medicalInformation.Drink,
                OtherDetails = medicalInformation.OtherDetails,
                CreatedBy = medicalInformation.CreatedBy,
                CreatedDate = DateTime.Now
            };
            return Ok(_medicalInformationRepository.Insert(_medicalInformation));
        }

        [HttpPost]
        [Route("api/user/update/medicalinfo/{ClientId}")]
        public IHttpActionResult UpdateMedicalInformation(string ClientId, MedicalInformation medicalInformation)
        {
            MedicalInformation mInfo = _medicalInformationRepository.Find(x => x.clientId == ClientId).FirstOrDefault();

            mInfo.Hight = medicalInformation.Hight;
            mInfo.Wight = medicalInformation.Wight;
            mInfo.BloodGroup = medicalInformation.BloodGroup;
            mInfo.Diseases = medicalInformation.Diseases;
            mInfo.AnyAllergies = medicalInformation.AnyAllergies;
            mInfo.AnyHealthCrisis = medicalInformation.AnyHealthCrisis;
            mInfo.AnyRegularMedication = medicalInformation.AnyRegularMedication;
            mInfo.Disability = medicalInformation.Disability;
            mInfo.Smoke = medicalInformation.Smoke;
            mInfo.Drink = medicalInformation.Drink;
            mInfo.OtherDetails = medicalInformation.OtherDetails;
            mInfo.ModifiedBy = medicalInformation.ModifiedBy;
            mInfo.ModifiedDate = DateTime.Now;
            return Ok(_medicalInformationRepository.Update(mInfo));
        }

        [HttpGet]
        [Route("api/user/get/medicalinfo/{ClientId}")]
        [AllowAnonymous]
        public IHttpActionResult getMedicalInformationt(string ClientId)
        {
           List<MedicalInformation> _miInformations = _medicalInformationRepository.Find(x => x.clientId == ClientId).OrderByDescending(x=>x.Id).Take(2).ToList();
            return Ok(_miInformations);
        }


        [HttpGet]
        [Route("api/user/get/insuranceinfo/{ClientId}")]
        public IHttpActionResult getInsurancenformationt(string ClientId)
        {
            InsuranceInformation _insuranceContact = _insuranceInformationRepository.Find(x => x.ClientId == ClientId).FirstOrDefault();
            return Ok(_insuranceContact);
        }
        [HttpGet]
        [Route("api/user/get/appointmentinsuranceinfo/{appointmentid}")]
        public IHttpActionResult getAppointmentInsurancenformationt(string appointmentid)
        {
            IAppointmentRepository _appointmentRepo = RepositoryFactory.Create<IAppointmentRepository>(ContextTypes.EntityFramework);
            var appointment = _appointmentRepo.Find(x => x.AppointmentId == appointmentid).FirstOrDefault();
            InsuranceInformation _insuranceContact = new InsuranceInformation();
            if (appointment!=null)
            {
                _insuranceContact = _insuranceInformationRepository.Find(x => x.Id == appointment.InsuranceId).FirstOrDefault();
            }
            return Ok(_insuranceContact);
        }
        [HttpGet]
        [Route("api/user/get/insurancedetail/{insuranceId}")]
        public IHttpActionResult getinsurancedetail(int insuranceId)
        {
            InsuranceInformation _insuranceContact = _insuranceInformationRepository.Find(x => x.Id == insuranceId).FirstOrDefault();
            return Ok(_insuranceContact);
        }

        [HttpGet]
        [Route("api/user/getall/insuranceinfo/{ClientId}")]
        public IHttpActionResult getallInsurancenformationt(string ClientId)
        {
            List<InsuranceInformation> _insuranceContact = _insuranceInformationRepository.Find(x => x.ClientId == ClientId).ToList();
            return Ok(_insuranceContact);
        }
        [HttpPost]
        [Route("api/user/add/insuranceinfo/{ClientId}")]
        [AllowAnonymous]
        public IHttpActionResult AddInsurancenformationt(string ClientId, InsuranceInformation insuranceInformation)
        {
            InsuranceInformation _insuranceInformation = new InsuranceInformation
            {
                ClientId = ClientId,
                CompanyName = insuranceInformation.CompanyName,
                InsuraceNo = insuranceInformation.InsuraceNo,
                ExpiryDate = insuranceInformation.ExpiryDate,
                IsActive = true
                
            };
            return Ok(_insuranceInformationRepository.Insert(_insuranceInformation));
        }

        [HttpPost]
        [Route("api/user/update/insuranceinfo")]
        public IHttpActionResult UpdateInsurancenformationt(InsuranceInformation insuranceInformation)
        {
            InsuranceInformation _insuranceInformation = _insuranceInformationRepository.Find(x => x.Id == insuranceInformation.Id).FirstOrDefault();
            _insuranceInformation.CompanyName = insuranceInformation.CompanyName;
            _insuranceInformation.InsuraceNo = insuranceInformation.InsuraceNo;
            _insuranceInformation.ExpiryDate = insuranceInformation.ExpiryDate;
            return Ok(_insuranceInformationRepository.Update(_insuranceInformation));
        }

        [HttpPost]
        [Route("api/user/delete/insuranceinfo")]
        public IHttpActionResult DeleteInsurancenformationt(InsuranceInformation insuranceInformation)
        {
            InsuranceInformation _insuranceInformation = _insuranceInformationRepository.Find(x => x.Id == insuranceInformation.Id).FirstOrDefault();
            _insuranceInformation.IsActive = insuranceInformation.IsActive;
            return Ok(_insuranceInformationRepository.Update(_insuranceInformation));
        }

        //New lead

        [HttpPost]
        [Route("api/user/LeadRegister")]
        [AllowAnonymous]
        public HttpResponseMessage LeadRegister(Lead objLead)
        {
           try
            {
                ILeadRepository _leadRepo = RepositoryFactory.Create<ILeadRepository>(ContextTypes.EntityFramework);
                objLead.DateEntered = DateTime.Now;
                 _leadRepo.Insert(objLead);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.Accepted, ex.Message);
            }

            return Request.CreateResponse(HttpStatusCode.Accepted,"success");
        }

    }
}
