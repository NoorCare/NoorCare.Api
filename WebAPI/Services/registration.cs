using AngularJSAuthentication.API.Services;
using NoorCare.Repository;
using System;
using System.Text;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class Registration
    {
        EmailSender _emailSender = new EmailSender();
        IHospitalDetailsRepository _hospitalDetailsRepository = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
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
                Type = model.Type,
                jobType = model.jobType,
                Mobile = Convert.ToInt64(model.PhoneNumber),
                FacilityId = model.FacilityId,
                Country = model.NationalityId.ToString(),
                ModifiedDate = DateTime.Now,
                CreatedDate = DateTime.Now,
                LimitBannerCount = 0,
                LimitVideoCount=0,
                LimitGallaryCount=0
            };
            return _hospitalDetailsRepository.Insert(_hospitalDetail);
        }

        public int AddFacilityDetail(string clientId, AccountModel model, IFacilityDetailRepository _facilityDetailsRepository)
        {
            FacilityDetail _facilityDetail = new FacilityDetail
            {
                FacilityDetailId = clientId,
                Email = model.Email,
                jobType = model.jobType,
                FacilityId = model.FacilityId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CountryCode = model.CountryCode,
                EmailConfirmed = model.EmailConfirmed,
                PhoneNumber = model.PhoneNumber,
                CreatedBy = "Self",
                ModifiedBy = "Self",
                DateEntered = DateTime.Now.ToString(),
                DateModified = DateTime.Now.ToString()

            };
            return _facilityDetailsRepository.Insert(_facilityDetail);
        }

        public string creatId(int jobType, string NationalityId, int? gender)
        {
            bool IsPriFix = false;
            string priFix = "NCM-";
            if (gender == 1 && jobType == 1)
            {
               priFix = "NCM-";
               IsPriFix = true;
            }
            else if (gender == 2 && jobType == 1 && IsPriFix ==false)
            {
                 priFix = "NCF-";
                IsPriFix = true;
            }
            else if (jobType == 3 && IsPriFix == false)
            {
                 priFix = "NCD-"; IsPriFix = true;
            }
            else if (jobType == 4 && IsPriFix == false)
            {
                 priFix = "NCS-"; IsPriFix = true;
            }
            else if (IsPriFix == false)
            {
                 priFix = "NCH-"; IsPriFix = true;
            }

            if (NationalityId.Length == 2)
            {
                NationalityId = "0" + NationalityId;
            }
            if (NationalityId.Length == 1)
            {
                NationalityId = "00" + NationalityId;
            }
            string clientId = priFix + NationalityId + "-" + _emailSender.Get();
            return clientId;


            //if (jobType == 3)
            //{
            //    priFix = "NCD-";
            //}
            //else if (jobType == 4)
            //{
            //    priFix = "NCS-";
            //}
            //else if (jobType == 5) // For Appointment
            //{
            //    priFix = "NCA-";
            //}
            //else if (jobType == 2)
            //{
            //    priFix = "NCH-";
            //}
            //else if (gender == 1 && jobType == 1)
            //{
            //    priFix = "NCM-";
            //}
            //else if (gender == 2 && jobType == 1)
            //{
            //    priFix = "NCF-";
            //}

            //else if (jobType == 6)//6   Medical Center
            //{
            //    priFix = "NCH-";
            //}
            //else if (jobType == 7) //7   Nursing Home
            //{
            //    priFix = "NCH-";
            //}
            //else if (jobType == 8) //8   Urgent Care Clinic
            //{
            //    priFix = "NCH-";
            //}
            //else if (jobType == 9)//9   Dental Care
            //{
            //    priFix = "NCH-";
            //}
            //else if (jobType == 9) //10  Medical Lab
            //{
            //    priFix = "NCH-";
            //}
            //else if (jobType == 11)//11  Pharmacy
            //{
            //    priFix = "NCH-";
            //}
            //else if (jobType == 12)//12  Blood Bank 
            //{
            //    priFix = "NCH-";
            //}
            //else if (jobType == 13)//13  Insurance Company
            //{
            //    priFix = "NCH-";
            //}
            //else if (jobType == 14)//14  X - Ray
            //{
            //    priFix = "NCH-";
            //}
            //else if (jobType == 15) //15  Optics
            //{
            //    priFix = "NCH-";
            //}





            //new Job Type
            //2   Hospital
            //5   Clinic
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

        public ApplicationUser UserAccount(dynamic model, int countrycodevalue)
        {
            var user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                JobType = model.jobType,
                CountryCodes = countrycodevalue,
                Gender = model.jobType == 1 ? model.Gender : 0,
            };
            user.FirstName = model.FirstName;
            user.PhoneNumber = model.PhoneNumber;
            user.LastName = model.LastName;
            if (model.HospitalId != null)
            {
                string hid = model.HospitalId;
                var NationalityId = hid.Split('-')[1];
                user.Id = creatId(user.JobType, NationalityId, user.Gender);
            }
            else
            {
                user.Id = creatId(user.JobType, model.NationalityId, user.Gender);
            }
            var dsid = user.Id.Split('-')[0];
            if (dsid == "NCD" || dsid == "NCS")
            {
                user.EmailConfirmed = true;
            }
            return user;
        }

        public ApplicationUser UserAcoount(dynamic model, int countrycodevalue)
        {
            var user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                JobType = model.jobType,
                CountryCodes = countrycodevalue,
                Gender = model.jobType == 1 ? model.Gender : 0,
            };
            user.FirstName = model.FirstName;
            user.PhoneNumber = model.PhoneNumber;
            user.LastName = model.LastName;
            if (model.HospitalId != null)
            {
                string hid = model.HospitalId;
                var NationalityId = hid.Split('-')[1];
                user.Id = creatId(user.JobType, NationalityId, user.Gender);
            }
            else
            {
                user.Id = creatId(user.JobType, model.NationalityId, user.Gender);
            }
            return user;
        }

        public void sendRegistrationEmail(ApplicationUser model)
        {
            try
            {
                _emailSender.email_send(model.Email, model.FirstName + " " + model.LastName == null ? "" : model.LastName, model.Id, model.JobType, model.PasswordHash);

               // string sMessage = "Welcome to NoorCare family(" + model.FirstName + ") this is your NoorCare number save it for further communication (" + model.NoorCareNumber + ") ";

              //  _emailSender.SendSMS(model.PhoneNumber, sMessage);

               // Welcome to NoorCare family(user name) this is your NoorCare number save it for further communication
               // (NoorCare number)

                //http://api.smscountry.com/SMSCwebservice_bulk.aspx?User=NoorCare&passwd=NoorCare@123&mobilenumber=97433977547&message=Hi Aslam This is test by Veerendra if you received this message plz let men know on whatsapp
                // &sid = Noorcare & mtype = N & DR = Y
            }
            catch (Exception ex)
            {
            }
        }

        public void sendRegistrationMessage(ApplicationUser model)
        {
            try
            {
                string sMessage = "Welcome to NoorCare family(" + model.FirstName + ") this is your NoorCare number save it for further communication (" + model.Id + ") ";
                _emailSender.SendSMS(model.PhoneNumber, sMessage);
            }
            catch (Exception ex)
            {
            }
        }

        public void sendForgotPassword(ApplicationUser model,string password)
        {
            try
            {

                string ClientName = "";//model.FirstName + " " + model.LastName == null ? "" : model.LastName;
                if (model.LastName==null)
                {
                    ClientName = model.FirstName;
                }
                else
                {
                    ClientName = model.FirstName + " "+ model.LastName;
                }
                _emailSender.email_sendforgotpassword(model.Email, ClientName, model.Id, password);
            }
            catch (Exception ex)
            {
                
            }
               
        }
    }
}