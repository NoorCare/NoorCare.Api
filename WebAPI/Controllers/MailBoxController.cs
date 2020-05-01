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
        [AllowAnonymous]
        // GET: MailBox
        public IHttpActionResult getMailBox(string ClientId, string lableName = null)
        {
            //IHospitalDetailsRepository _hospitalRepo = RepositoryFactory.Create<IHospitalDetailsRepository>(ContextTypes.EntityFramework);
            List<MailBox> mailBox = new List<MailBox>();
            if (lableName != null && (lableName == "Send"|| lableName=="Draft"))
            {
                mailBox = _mailbocRepository.Find(
                   x => x.EmailFrom == ClientId &&
                    lableName == "Draft" ? x.LabelName == lableName : x.LabelName != "Draft");


            }
            else
            {
                mailBox = _mailbocRepository.Find(
                    x => x.EmailTo == ClientId
                    &&
                    lableName != null ? x.LabelName == lableName : x.EmailTo == ClientId
                    );

            }
            return Ok(mailBox);
        }

        [HttpPost]
        [Route("api/mailbox/add/{ClientId}")]
        [AllowAnonymous]
        // GET: MailBox
        public IHttpActionResult addMailBox(string ClientId, MailBox mailBox)
        {
            return Ok(_mailbocRepository.Insert(mailBox));
        }

        [HttpPost]
        [Route("api/mailbox/update/{ClientId}")]
        [AllowAnonymous]
        // GET: MailBox
        public IHttpActionResult updateMailBox(string ClientId, MailBox mailBox)
        {
            return Ok(_mailbocRepository.Update(mailBox));
        }
    }
}