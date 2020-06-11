using AngularJSAuthentication.API.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
using WebAPI.Entity;
using WebAPI.Models;
using WebAPI.Repository;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    public class EnquiryController : ApiController
    {
        IEnquiryRepository _enquiryRepository = RepositoryFactory.Create<IEnquiryRepository>(ContextTypes.EntityFramework);

        [Route("api/enquiry/getall")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetAll()
        {
            var result = _enquiryRepository.GetAll().ToList();
            return Ok(result);
        }


        [Route("api/enquiry/add")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult add([FromBody]Enquiry enquiry)
        {
            if (enquiry!=null)
            {
                //insert 
                enquiry.CreatedBy = enquiry.Name;
                enquiry.CreatedDate = DateTime.Now;
                var _enquiryCreated = _enquiryRepository.Insert(enquiry);
                return Ok(_enquiryCreated);
            }
            return InternalServerError();
        }


    }
}
