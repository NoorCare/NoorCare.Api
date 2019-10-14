using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;
using WebAPI;

namespace AngularJSAuthentication.API.Services
{
    public class EmailSender
    {
        private WebProxy objProxy1 = null;

        string SMTP = ConfigurationManager.AppSettings.Get("SMTP");
        string SMTPUserId = ConfigurationManager.AppSettings.Get("SMTPUserId");
        string SMTPPassword = ConfigurationManager.AppSettings.Get("SMTPPassword");
        string From = ConfigurationManager.AppSettings.Get("From");
        string SMTPPORT = ConfigurationManager.AppSettings.Get("SMTPPORT");

        string SMSUserId = ConfigurationManager.AppSettings.Get("SMSUserId");
        string SMSPassword = ConfigurationManager.AppSettings.Get("SMSPassword");
        string SMSSid = ConfigurationManager.AppSettings.Get("SMSSid");

        public void email_send(string mailTo = "NoorCareNew@gmail.com", string clientName = "Noor Care New", 
            string ClientId = "Test", int jobType = 0, string password = null, string PhoneNumber="", string CustomMessage="")
        {
            string prifix = jobType == 3 ? "Dr " : "";
            string html = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Services/templat.html"));
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(SMTP);
            //mail.From = new MailAddress("NoorCareNew@gmail.com");
            mail.From = new MailAddress(From);
            mail.To.Add(mailTo);
            mail.IsBodyHtml = true;
            mail.Subject = "Registration Successfully ";
            mail.Body = html.Replace("CLIENTNAME", prifix + clientName + "("+ ClientId + ")");
            mail.Body = getLogoUrl(mail.Body);
            mail.Body = getVereficationUrl(mail.Body, ClientId);
            mail.Body = tempPassword(mail.Body, password, jobType);
            SmtpServer.Port = 587;
            SmtpServer.EnableSsl = true;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new NetworkCredential(From, SMTPPassword);
            SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            SmtpServer.Send(mail);

            // Sent SMS
            try
            {
                SendSMS(SMSUserId, SMSPassword, PhoneNumber, CustomMessage + DateTime.Now, "N", "Y", SMSSid);
            }
            catch (Exception ex)
            {
            }
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

        public string tempPassword(string html, string password, int jobTpye)
        {
            return html.Replace("PASSWORD", jobTpye == 4 || jobTpye == 3 ? $"<p>Your First Time Password is <b>{password}</b></p>" : "");
        }

        public string getVereficationUrl(string html, string clientId)
        {
            return html.Replace("VERFICATIONSRC",
               "<a href='"+ constant.emailidVerefactionUrl.Replace("CLIENTID", clientId) + "' VERFICATIONSRC style = 'display:inline-block;text-decoration:none;color:#ffffff;font-size:15px;font-family:ArialMT, Arial, sans-serif;font-weight:bold;text-align:center;width:100%;' >Confirm Account </ a > "
               );
        }

        #region SMS Code
               

        public string SendSMS(string User, string password, string Mobile_Number, string Message, string Mtype, string DR, string SID)
        {

            System.Object stringpost = "User=" + User + "&passwd=" + password + "&mobilenumber=" + Mobile_Number + "&message=" + Message + "&MType=" + Mtype + "&DR=" + DR + "&SID=" + SID;

            //string functionReturnValue = null;
            //functionReturnValue = "";

            HttpWebRequest objWebRequest = null;
            HttpWebResponse objWebResponse = null;
            StreamWriter objStreamWriter = null;
            StreamReader objStreamReader = null;

            try
            {
                string stringResult = null;

                objWebRequest = (HttpWebRequest)WebRequest.Create("http://www.smscountry.com/SMSCwebservice_bulk.aspx?");
                objWebRequest.Method = "POST";

                if ((objProxy1 != null))
                {
                    objWebRequest.Proxy = objProxy1;
                }

                // Use below code if you want to SETUP PROXY.
                //Parameters to pass: 1. ProxyAddress 2. Port
                //You can find both the parameters in Connection settings of your internet explorer.

                //WebProxy myProxy = new WebProxy("YOUR PROXY", PROXPORT);
                //myProxy.BypassProxyOnLocal = true;
                //wrGETURL.Proxy = myProxy;

                objWebRequest.ContentType = "application/x-www-form-urlencoded";

                objStreamWriter = new StreamWriter(objWebRequest.GetRequestStream());
                objStreamWriter.Write(stringpost);
                objStreamWriter.Flush();
                objStreamWriter.Close();

                objWebResponse = (HttpWebResponse)objWebRequest.GetResponse();
                objStreamReader = new StreamReader(objWebResponse.GetResponseStream());
                stringResult = objStreamReader.ReadToEnd();

                objStreamReader.Close();
                return (stringResult);
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }
            finally
            {

                if ((objStreamWriter != null))
                {
                    objStreamWriter.Close();
                }
                if ((objStreamReader != null))
                {
                    objStreamReader.Close();
                }
                objWebRequest = null;
                objWebResponse = null;
                objProxy1 = null;
            }
        }

        #endregion

    }
}