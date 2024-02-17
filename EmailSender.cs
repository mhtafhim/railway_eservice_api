using Newtonsoft.Json.Linq;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Model;
using System.Diagnostics;

namespace railway_eservice_api
{
    public class EmailSender
    {

        public void SendEmail(string toEmail, string toName, string message)
        {
            var apiInstance = new TransactionalEmailsApi();
            string SenderName = "Railway eService Authority";
            string SenderEmail = "mahmudulhoquetafhim@gmail.com";
            SendSmtpEmailSender Email = new SendSmtpEmailSender(SenderName, SenderEmail);
            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(toEmail, toName);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            To.Add(smtpEmailTo);

           
            string HtmlContent =null;
            string TextContent = message;
            string Subject = "Railway eService OTP ";
           
            try
            {
                var sendSmtpEmail = new SendSmtpEmail(Email, To, null, null, HtmlContent, TextContent, Subject);
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

    }
}
