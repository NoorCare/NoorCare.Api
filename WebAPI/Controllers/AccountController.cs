﻿using AngularJSAuthentication.API.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class AccountController : ApiController
    {

        IClientDetailRepository clientDetailRepo = RepositoryFactory.Create<IClientDetailRepository>(ContextTypes.EntityFramework);
        EmailSender _emailSender = new EmailSender();

        [Route("api/account/register")]
        [HttpPost]
        [AllowAnonymous]
        public IdentityResult Register(AccountModel model)
        {
           
            var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var manager = new UserManager<ApplicationUser>(userStore);
            string clientId = model.Gender == 1 ? "NCM974-" + _emailSender.Get() : "NCF974-" + _emailSender.Get();
            var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email };
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Id = clientId;
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 3
            };
            IdentityResult result = manager.Create(user, model.Password);

            ClientDetail _clientDetail = new ClientDetail
            {
                ClientId = clientId,
                Name = model.UserName,
                Gender = model.Gender,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Country = model.Country,
                MobileNo = model.PhoneNumber,
                EmailId = model.Email,
                Jobtype = model.jobType,
                CreatedDate = DateTime.Now
            };
            clientDetailRepo.Insert(_clientDetail);
            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return null;
            }
            else
            {
                _emailSender.email_send(model.Email, clientId);
            }
            return result;
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        [HttpGet]
        [Route("api/GetUserClaims")]
        public AccountModel GetUserClaims()
        {
            var identityClaims = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identityClaims.Claims;
            AccountModel model = new AccountModel()
            {
                UserName = identityClaims.FindFirst("Username").Value,
                Email = identityClaims.FindFirst("Email").Value,
                FirstName = identityClaims.FindFirst("FirstName").Value,
                LastName = identityClaims.FindFirst("LastName").Value,
                LoggedOn = identityClaims.FindFirst("LoggedOn").Value
            };
            return model;
        }

        //[HttpGet]
        //[Route("api/GetUserClaims")]
        //public bool GetUserNameEmailIdExit(AccountModel model)
        //{
        //    //public virtual Task<TUser> FindByEmailAsync(string email);
        //    //public virtual Task<TUser> FindByIdAsync(TKey userId);
        //    //public virtual Task<TUser> FindByNameAsync(string userName);
        //    var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
        //    var manager = new UserManager<ApplicationUser>(userStore);
        //var user = await manager.FindByEmailAsync(model.Email);
        //   return clientDetailRepo.Find(p => p.EmailId )
        //}
    }
}