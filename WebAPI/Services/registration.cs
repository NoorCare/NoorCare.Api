using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class Registration
    {
        public int AddClientDetail(string clientId, AccountModel model, IClientDetailRepository _clientDetailRepo)
        {
            ClientDetail _clientDetail = new ClientDetail
            {
                ClientId = clientId,
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                MobileNo = Convert.ToInt32(model.PhoneNumber),
                EmailId = model.Email,
                Jobtype = model.jobType,
                CountryCode = model.CountryCode,
                CreatedDate = DateTime.Now,
            };
           return _clientDetailRepo.Insert(_clientDetail);
        }

        public int AddHospitalDetail(string clientId, AccountModel model, IHospitalDetailsRepository _hospitalDetailsRepository)
        {
            HospitalDetail _hospitalDetail = new HospitalDetail
            {
                HospitalId = clientId,
                HospitalName = model.FirstName,
                Email = model.Email,
                jobType = model.jobType,
                Mobile = Convert.ToInt32(model.PhoneNumber),
                FacilityId = model.FacilityId
            };
            return _hospitalDetailsRepository.Insert(_hospitalDetail);
        }

        public string creatIdPrix(AccountModel model)
        {
            string priFix = "NCM-";
            if (model.jobType == 2)
            {
                priFix = "NCH-";
            }
            else if (model.Gender == 1 && model.jobType == 1)
            {
                priFix = "NCM-";
            }
            else if (model.Gender == 2 && model.jobType == 1)
            {
                priFix = "NCF-";
            }
            return priFix;
        }
    }
}