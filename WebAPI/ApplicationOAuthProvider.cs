using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        IClientDetailRepository _clientDetailRepo = RepositoryFactory.Create<IClientDetailRepository>(ContextTypes.EntityFramework);
        IHospitalDetailsRepository _hospitalDetailsRepository = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
        IDoctorRepository _doctorRepository = RepositoryFactory.Create<IDoctorRepository>(ContextTypes.EntityFramework);
        ISecretaryRepository _secretaryRepository = RepositoryFactory.Create<ISecretaryRepository>(ContextTypes.EntityFramework);

        ClientDetail clientDetailRepo = null;
        HospitalDetails hospitalDetails;
        Doctor doctor;
        Secretary  secretary;
        int jobType = 1;
        bool isEmailConfirmed = false;
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            var userFindByEmail = manager.FindByEmail(context.UserName);
            var user = userFindByEmail != null ? await manager.FindAsync(userFindByEmail.UserName, context.Password)
                : await manager.FindAsync(context.UserName, context.Password);
            if (user == null)
            {
                context.SetError("Please check username and password");
            }
            else
            {
                if (user.JobType == 2) {
                    hospitalDetails = _hospitalDetailsRepository.Find(x => x.HospitalId.Trim() == user.Id.Trim()).FirstOrDefault();
                    jobType = hospitalDetails.jobType;
                    isEmailConfirmed = hospitalDetails.EmailConfirmed;
                }
                else  if (user.JobType == 1) {
                    clientDetailRepo = _clientDetailRepo.Find(x => x.ClientId.Trim() == user.Id.Trim()).FirstOrDefault();
                    jobType = clientDetailRepo.Jobtype;
                    isEmailConfirmed = clientDetailRepo.EmailConfirmed;
                }
                else if (user.JobType == 3)
                {
                    doctor = _doctorRepository.Find(x => x.DoctorId.Trim() == user.Id.Trim()).FirstOrDefault();
                    jobType = doctor.jobType;
                    isEmailConfirmed =true;
                }
                else if (user.JobType == 4)
                {
                    secretary = _secretaryRepository.Find(x => x.SecretaryId.Trim() == user.Id.Trim()).FirstOrDefault();
                    jobType = secretary.jobType;
                    isEmailConfirmed = true;
                }
                if (!isEmailConfirmed)
                {
                    context.SetError("Please verify your email address");
                }
                else
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("UserId", user.Id));
                    identity.AddClaim(new Claim("Username", user.UserName));
                    identity.AddClaim(new Claim("Email", user.Email));
                    identity.AddClaim(new Claim("FirstName", user.FirstName));
                    identity.AddClaim(new Claim("LastName", user.LastName == null? "" : user.LastName));
                    identity.AddClaim(new Claim("LoggedOn", DateTime.Now.ToString()));
                    identity.AddClaim(new Claim("PhoneNo", user.PhoneNumber == null? " " : user.PhoneNumber));
                    identity.AddClaim(new Claim("JobType", user.JobType.ToString()));
                    context.Validated(identity);
                }
            }
        }
    }
}