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
    public class MailBoxController : ApiController
    {
        IMailBoxRepository _mailbocRepository =
            RepositoryFactory.Create<IMailBoxRepository>(ContextTypes.EntityFramework);

        IMailBoxAttachmentsRepository _mailboxAttachmentsRepository =
            RepositoryFactory.Create<IMailBoxAttachmentsRepository>(ContextTypes.EntityFramework);

        [HttpGet]
        [Route("api/mailbox/{ClientId}/{lableName?}")]
        // GET: MailBox
        public HttpResponseMessage getMailBox(string ClientId, string lableName = null)
        {
            //IHospitalDetailsRepository _hospitalRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
            List<MailBox> mailBox = _mailbocRepository.Find(
                x => x.EmailTo == ClientId &&
                lableName != null ? x.LabelName == lableName : x.LabelName == x.LabelName);


            return Request.CreateResponse(HttpStatusCode.Accepted, mailBox);
        }
    }
}