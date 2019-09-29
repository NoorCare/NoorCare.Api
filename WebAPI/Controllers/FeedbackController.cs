using NoorCare.Repository;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Entity;
using WebAPI.Repository;

namespace WebAPI.Controllers
{
    public class FeedbackController : ApiController
    {
        IFeedbackRepository _feedbackRepo = RepositoryFactory.Create<IFeedbackRepository>(ContextTypes.EntityFramework);
        IFeedbackRepository _feedbackList = RepositoryFactory.Create<IFeedbackRepository>(ContextTypes.EntityFramework);
        IContactUsRepository _contactUsRepo = RepositoryFactory.Create<IContactUsRepository>(ContextTypes.EntityFramework);

        [Route("api/feedback/getAllFeedback")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/Feedback
        public HttpResponseMessage GetAllFeedback()
        {            
            var result = _feedbackRepo.GetAll().ToList();

            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/feedback/getdetail/{feedbackId}/{pageId}")]
        [HttpGet]
        [AllowAnonymous]
        // GET: api/feedback/5
        public HttpResponseMessage GetDetail(string feedbackId, string pageId)
        {
            var result = _feedbackRepo.Find(x => x.FeedbackID == feedbackId && x.PageId== pageId).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);            
        }

        [Route("api/feedback/register")]
        [HttpPost]
        [AllowAnonymous]
        // POST: api/feedback
        public HttpResponseMessage Register(Feedback obj)
        {
            if(ModelState.IsValid)
            {
                Random rd = new Random(987612345);
                var _feedbackId ="F_"+ rd.Next();
                obj.FeedbackID = _feedbackId;
                var _feedbackCreated = _feedbackRepo.Insert(obj);
                return Request.CreateResponse(HttpStatusCode.Accepted, obj.FeedbackID);
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest,"");
        }


        [Route("api/feedback/update")]
        [HttpPut]
        [AllowAnonymous]
        // PUT: api/Feedback/5
        public HttpResponseMessage Update(Feedback obj)
        {
            int tbleId = getTableId(obj.FeedbackID);
            obj.Id = tbleId;
            var result = _feedbackRepo.Update(obj);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/feedback/delete/{feedbackid}")]
        [HttpDelete]
        [AllowAnonymous]
        // DELETE: api/Feedback/5
        public HttpResponseMessage Delete(string feedbackid)
        {
            int tbleId= getTableId(feedbackid);

            var result = _feedbackRepo.Delete(tbleId);
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        private int getTableId(string feedbackId)
        {
            IFeedbackRepository _doctorRepo = RepositoryFactory.Create<IFeedbackRepository>(ContextTypes.EntityFramework);
            var result = _doctorRepo.Find(x => x.FeedbackID == feedbackId).FirstOrDefault();

            return result.Id;
        }

        //ContactUs from Doctor Details/ Hospital Details
        [Route("api/feedback/contactUs")]
        [HttpPost]
        [AllowAnonymous]
        // POST: api/feedback
        public HttpResponseMessage SaveContactUs(ContactUs obj)
        {          
            var _contactUsCreated = _contactUsRepo.Insert(obj);
            //var msg = "Thank You for contacting NoorCare. Our representative will get back to you within 24 hours.";
            //string uri = "";
            //sendSMS(uri);

            return Request.CreateResponse(HttpStatusCode.Accepted, obj.Id);
        }

        [Route("api/GetAllContactUs")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetAllContactUs()
        {
            var result = _contactUsRepo.GetAll().ToList();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/GetContactUs/{Id}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetContactUs(int Id)
        {
            var result = _contactUsRepo.Find(x =>x.Id == Id).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        [Route("api/DeleteContactUs/{Id}")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage DeleteContactUs(int Id)
        {
            var result = _contactUsRepo.Find(x => x.Id == Id).FirstOrDefault().IsDeleted= true;
            
            return Request.CreateResponse(HttpStatusCode.Accepted, result);
        }

        private void sendSMS(string uri)
        {
            string response = string.Empty;

            HttpWebRequest req = WebRequest.Create(new Uri(uri)) as HttpWebRequest;
            req.KeepAlive = false;
            req.Method = "GET";
            req.ContentType = "application/json";
            try
            {
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
                using (StreamReader loResponseStream = new StreamReader(resp.GetResponseStream())) //, enc
                {
                    response = loResponseStream.ReadToEnd();
                    loResponseStream.Close();
                    resp.Close();
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }

    }
}
