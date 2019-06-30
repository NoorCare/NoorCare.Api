using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using WebAPI;

namespace AngularJSAuthentication.API.Services
{
    public class EmailSender
    {
        public void email_send(string mailTo = "manishcs0019@gmail.com", string clientName = "Manish Sharma", 
            string ClientId = "Test")
        {
            string html = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Services/templat.html"));
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("noorcare2019@gmail.com");
            mail.To.Add(mailTo);
            mail.IsBodyHtml = true; //to make message body as html  
            mail.Subject = "Registration Successfully ";
            //for (int i = 10; i <= ClientId.Length; i += 3)
            //{
            //    ClientId = i == 15 ? ClientId : ClientId.Insert(i, "-");
            //    i++;
            //}
            mail.Body = html.Replace("CLIENTNAME", clientName +"("+ ClientId + ")");
             
            mail.Body = getLogoUrl(mail.Body);
            mail.Body = getVereficationUrl(mail.Body, ClientId);
            SmtpServer.Port = 587;
            SmtpServer.EnableSsl = true;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new NetworkCredential("noorcare2019@gmail.com", "NoorCare@123");
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpServer.Send(mail);

        }

        private int _InternalCounter = 0;

        public string Get()
        {
            var now = DateTime.Now;

            var days = (int)(now - new DateTime(2000, 1, 1)).TotalDays;
            var seconds = (int)(now - DateTime.Today).TotalSeconds;

            var counter = _InternalCounter++ % 100;
            string result = days.ToString("00000") + seconds.ToString("00000") + counter.ToString("00");
            for (int i = 3; i <= result.Length; i += 3)
            {
                result = i == result.Length ? result : result.Insert(i, "-");
                i++;
            }
            return result;
        }

        public string getLogoUrl(string html)
        {
            return html.Replace("LOGOSRC", 
               "<img src='"+ constant.logoUrl +"' style = 'border:0;width:200px;max-width:100%;' alt = 'Header' title = 'Image' />"
               );
        }

        public string getVereficationUrl(string html, string clientId)
        {
            return html.Replace("VERFICATIONSRC",
               "<a href='"+ constant.emailidVerefactionUrl.Replace("CLIENTID", clientId) + "' VERFICATIONSRC style = 'display:inline-block;text-decoration:none;color:#ffffff;font-size:15px;font-family:ArialMT, Arial, sans-serif;font-weight:bold;text-align:center;width:100%;' >Confirm Account </ a > "
               );
        }

    }
}