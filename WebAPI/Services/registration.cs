using AngularJSAuthentication.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class Registration
    {
        EmailSender _emailSender = new EmailSender();
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
            HospitalDetails _hospitalDetail = new HospitalDetails
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

        public string creatId(int jobType, int CountryCodes, int? gender)
        {
            string priFix = "NCM-";
            if (jobType == 3)
            {
                priFix = "NCD-";
            }
            else if (jobType == 4)
            {
                priFix = "NCS-";
            }
            else if (jobType == 5) // For Appointment
            {
                priFix = "NCA-";
            }
            else if (jobType == 2)
            {
                priFix = "NCH-";
            }
            else if (gender == 1 && jobType == 1)
            {
                priFix = "NCM-";
            }
            else if (gender == 2 && jobType == 1)
            {
                priFix = "NCF-";
            }
            string clientId = priFix + CountryCodes + "-" + _emailSender.Get();
            return clientId;
        }

        // Generate a random password of a given length (optional)  
        public string RandomPassword(int size = 0)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, true));
            builder.Append(RandomNumber(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        // Generate a random number between two numbers    
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        // Generate a random string with a given size and case.   
        // If second parameter is true, the return string is lowercase  
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public ApplicationUser UserAcoount(dynamic model, int countrycodevalue)
        {
            var user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                JobType = model.jobType,
                CountryCodes = countrycodevalue,
                Gender = model.jobType == 1 ? model.Gender : 0
            };
            user.FirstName = model.FirstName;
            user.PhoneNumber = model.PhoneNumber;
            user.LastName = model.LastName;
            user.Id = creatId(user.JobType, user.CountryCodes, user.Gender);
            return user;
        }

        public void sendRegistrationEmail(ApplicationUser model)
        {
            try
            {
                _emailSender.email_send(model.Email, model.FirstName + " "+ model.LastName == null ? "" : model.LastName, model.Id, model.JobType, model.PasswordHash);
            }
            catch (Exception ex)
            {

            }
        }
    }
}