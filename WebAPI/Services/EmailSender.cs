﻿using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace AngularJSAuthentication.API.Services
{
    public class EmailSender
    {
        public void email_send(string tokenCode, string mailTo = "manishcs0019@gmail.com",
            string clientName = "Manish Sharma", string ClientId = "Test")
        {
            string html = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Services/templat.html"));
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("noorcare2019@gmail.com");
            mail.To.Add(mailTo);
            mail.IsBodyHtml = true; //to make message body as html  
            mail.Subject = "Registration Successfully ";
            mail.Body = getHtml(clientName, ClientId);
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

        public string getHtml(string name, string cilentid)
        {

            string htmlEmail = @"< table align = 'center' style = 'background-color:#e9ebee;width:100%;max-width:100%;min-width:100%;' border = '0' cellpadding = '0' cellspacing = '0' width = '100%' >
< tr align = 'center' >< td style = 'width:100%;height:32px' >
</ td ></ tr >
< tr align = 'center' >
< td align = 'center' >
< table border = '0' cellpadding = '0' cellspacing = '0' style = 'background-color:#e9ebee;min-width:360px;max-width:600px;width:100%;' width = '100%' >
< table align = 'center' border = '0' cellpadding = '0' cellspacing = '0'
style = 'display:none; font-size:1px; color:#333333; line-height:1px; max-height:0px; max-width:0px; opacity:0; overflow:hidden;' width = '100%' >
< tr >< td ></ td ></ tr ></ table >
< table align = 'center' style = 'max-width:600px;width:100%;' cellpadding = '0' cellspacing = '0' border = '0' >
< tr align = 'center' >< td
style = 'max-width:600px;padding:50px 40px 40px 40px;width:100%;background-color:#fbfbfc;padding:0;background-color:#e9ebee;' >
< table align = 'center' border = '0' cellpadding = '0' cellspacing = '0' style = 'width:100%;' >< tr align = 'center' >< td style = 'text-align:center;border-spacing:0;color:#4c4c4c;font-family:ArialMT, Arial, sans-serif;font-size:15px;width:100%;' >
< img src = 'https://noorcare-31771.firebaseapp.com/assets/images/logos/Logo.png'
style = 'border:0;width:200px;max-width:100%;' alt = 'Header' title = 'Image' />
</ td ></ tr >
< tr align = 'center' >< td style = 'width:100%;height:0;' ></ td ></ tr ></ table >
</ td ></ tr >< tr align = 'center' >< td style = 'width:100%;height:26px;' ></ td ></ tr ></ table >
< table align = 'center' style = 'max-width:600px;width:100%;' cellpadding = '0' cellspacing = '0' border = '0' >< tr align = 'center' >
< td style = 'max-width:600px;padding:50px 40px 40px 40px;width:100%;background-color:#fbfbfc;background-color:#ffffff;background-image:linear-gradient(#ffffff,#edf2fa);border-radius:0 0 5px 5px;' >
< table align = 'center' border = '0' cellpadding = '0' cellspacing = '0' style = 'width:100%;' >
< tr align = 'center' >< td style = 'text-align:center;border-spacing:0;color:#4c4c4c;font-family:ArialMT, Arial, sans-serif;font-size:15px;width:100%;text-align:center;color:#333333;font-size:27px;font-family:ArialMT, Arial, sans-serif;font-weight:light;line-height:36px;' >
Welcome to the NoorCare Family</ td ></ tr >
< tr align = 'center' >< td style = 'width:100%;height:30px;' >
</ td ></ tr ></ table >
< table align = 'center' border = '0' cellpadding = '0' cellspacing = '0' style = 'width:100%;' >
< tr align = 'center' >< td style = 'text-align:left;border-spacing:0;color:#4c4c4c;font-family:ArialMT, Arial, sans-serif;font-size:15px;width:100%;line-height:24px;' >";

            htmlEmail += "< p > Hi " + name;

            htmlEmail += @",</ p >< p >< p >
Welcome to NoorCare and thanks for signing up!You're one step closer 
to purchasing the finest goods that we have to offer </ p ></ p ></ td ></ tr >
< tr align = 'center' >< td style = 'width:100%;height:30px;' ></ td ></ tr ></ table >
< table align = 'center' border = '0' cellpadding = '0' cellspacing = '0' style = 'width:100%;' >
< tr align = 'center' >< td style = 'text-align:center;border-spacing:0;color:#4c4c4c;font-family:ArialMT, Arial, sans-serif;font-size:15px;width:100%;width:100%;min-width:360px' >
< table align = 'center' style = 'background-color:#0C492A;border-radius:3.6px;padding:10px 30px;-webkit-border-radius:3.6px;-moz-border-radius:3.6px;' >< tr align = 'center' >
< td >";

            htmlEmail += "< a href = 'https://noorcare.qatarvisiting.com/api/" + cilentid + "'";
            htmlEmail += @"style = 'display:inline-block;text-decoration:none;color:#ffffff;font-size:15px;font-family:ArialMT, Arial, sans-serif;font-weight:bold;text-align:center;width:100%;' >
Confirm Account
</ a ></ td ></ tr >
</ table ></ td ></ tr >< tr align = 'center' >< td style = 'width:100%;height:0px;' ></ td ></ tr >
</ table ></ td ></ tr >< tr align = 'center' >
< td style = 'width:100%;height:5px;' ></ td ></ tr ></ table >
< table style = 'width:100%;' align = 'center' >
< tr align = 'center' >
< td >
< table align = 'center'
style = 'max-width:600px;width:100%;'
cellpadding = '0' cellspacing = '0' border = '0' >< tr align = 'center' >
< td style = 'max-width:600px;padding:50px 40px 40px 40px;width:100%;background-color:#fbfbfc;padding:20px 0px 20px 0px;background-color:#0C492A;' >< table align = 'center' border = '0' cellpadding = '0' cellspacing = '0' style = 'width:100%;' >< tr align = 'center' >
< td style = 'text-align:center;border-spacing:0;color:#4c4c4c;font-family:ArialMT, Arial, sans-serif;font-size:15px;width:100%;color:#fff;font-size:12px;padding:0 20px;' >
If you don &#039;t want to receive these emails from NoorCare in the future, please 
< a href = '#' style = 'color:#ccc;text-decoration:none;' > Unsubscribe here </ a >.</ td ></ tr ></ table >
</ table >
</ td >
</ tr >
< tr align = 'center' >< td style = 'width:100%;height:32px' ></ td ></ tr >
</ table >";
            return htmlEmail;
        }


    }
}