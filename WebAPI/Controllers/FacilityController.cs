using NoorCare.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Entity;

namespace WebAPI.Controllers
{
    public class FacilityController : ApiController
    {
        IFacilityRepository _facilityDetailRepo = RepositoryFactory.Create<IFacilityRepository>(
            ContextTypes.EntityFramework);

        IDiseaseRepository _diseaseDetailRepo = RepositoryFactory.Create<IDiseaseRepository>(
            ContextTypes.EntityFramework);

        [Route("api/facility")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetFacility()
        {
            return Ok(_facilityDetailRepo.GetAll());
        }

        [Route("api/diseaseType")]
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetDisease()
        {
            return Ok(_diseaseDetailRepo.GetAll());
        }

    }
}
