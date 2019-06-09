using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace AngularJSAuthentication.API.Services
{
    public class EmailSender
    {
        public void email_send(string mailTo = "manishcs0019@gmail.com", string clientName = "NCM974-070986464400")
        {
            string html = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Services/templat.html"));
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("noorcare2019@gmail.com");
            mail.To.Add(mailTo);
            mail.IsBodyHtml = true; //to make message body as html  
            mail.Subject = "Registration Successfully ";
            mail.Body = html.Replace("[[UserName]]", clientName);
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
            return result;
        }
        }




}