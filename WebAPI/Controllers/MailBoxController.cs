using Newtonsoft.Json.Linq;
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
        [Route("api/mailboxAttachments/get/{MailBoxId}")]
        public IHttpActionResult getMailBoxAttachments(int MailBoxId)
        {
            List<MailBoxAttachments> mailBoxAttachments = _mailboxAttachmentsRepository.Find(x => x.MailBoxId == MailBoxId).ToList();
            return Ok(mailBoxAttachments);
        }



        [HttpGet]
        [Route("api/mailbox/get/{ClientId}/{lableName?}")]
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
        [Route("api/mailbox/add")]
        [AllowAnonymous]
        // GET: MailBox
        public IHttpActionResult addMailBox([FromBody]JObject data)
        {
            try
            {
                MailBox mailBox = data["MailBox"].ToObject<MailBox>();
                MailBoxAttachments[] mailBoxAttachments = data["MailBoxAttachments"].ToObject<MailBoxAttachments[]>();
                int mailBoxid = 0;
                if (mailBox != null)
                {
                    if (mailBox.LabelName == "Draft")
                    {
                        mailBoxid = _mailbocRepository.Insert(mailBox);
                        if (mailBoxAttachments != null)
                        {
                            foreach (MailBoxAttachments element in mailBoxAttachments)
                            {
                                element.MailBoxId = mailBoxid;
                                _mailboxAttachmentsRepository.Insert(element);
                            }
                        }
                    }
                    else
                    {
                        string all_recp = mailBox.AllRecipients;
                        string[] emailTo = all_recp.Split(';');
                        foreach (var item in emailTo)
                        {
                            mailBox.EmailTo = item;
                            mailBoxid = _mailbocRepository.Insert(mailBox);
                            if (mailBoxAttachments != null)
                            {
                                foreach (MailBoxAttachments element in mailBoxAttachments)
                                {
                                    element.MailBoxId = mailBoxid;
                                    _mailboxAttachmentsRepository.Insert(element);
                                }
                            }
                        }
                    }
                   

                }
                return Ok(mailBoxid);

            }catch(Exception ex)
            {
                return InternalServerError(ex);
            }
            //return Ok(mailBoxid);
        }

        [HttpPost]
        [Route("api/mailbox/update")]
        [AllowAnonymous]
        // GET: MailBox
        public IHttpActionResult updateMailBox([FromBody]JObject data)
        {
            try
            {
                MailBox mailBox = data["MailBox"].ToObject<MailBox>();
                MailBoxAttachments[] mailBoxAttachments = data["MailBoxAttachments"].ToObject<MailBoxAttachments[]>();
                Boolean status=false;
                if (mailBox != null)
                {
                    status = _mailbocRepository.Update(mailBox);
                    int mailBoxid = mailBox.Id;
                    if (mailBoxAttachments != null)
                    {
                        foreach (MailBoxAttachments element in mailBoxAttachments)
                        {
                            if(element.MailBoxId==0)
                            {
                                element.MailBoxId = mailBoxid;
                                _mailboxAttachmentsRepository.Insert(element);
                            }
                            else
                            {
                                _mailboxAttachmentsRepository.Update(element);
                            }
                            
                        }
                    }
                }
                return Ok(status);

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/mailbox/delete")]
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult deleteMailBox(int[] Id)
        {
            if (Id != null)
            {
                foreach (int mailBoxId in Id)
                {
                    MailBox _mailBoxDtls = _mailbocRepository.Find(x => x.Id == mailBoxId).FirstOrDefault();
                    if (_mailBoxDtls != null)
                    {
                        _mailBoxDtls.EmailStatus = "Deleted";
                        var result = _mailbocRepository.Update(_mailBoxDtls);
                        return Ok(result);
                    }
                }
               
            }

            return Ok();
        }

        
    }
}