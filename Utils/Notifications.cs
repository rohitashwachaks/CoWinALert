using System;
//-----------------------------------
// using Twilio;
// using Twilio.Rest.Api.V2010.Account;
// using Twilio.Types;
//-----------------------------------
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using CoWinAlert.DTO;

namespace CoWinAlert.Utils
{
    public static class Notifications
    {
        #region Private Members
        private static SendGridClient client;
        private static EmailAddress sender;
        private static string EMAIL_SUBJECT = "CoWin Alert";

        #endregion Private Members
        #region Private Functions and Initialiser
        private static EmailAddress StructureEmailID(string userName, string emailId) => new EmailAddress(email: emailId, name: userName);
        
        public static void Initialise()
        {
            string emailApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            client = new SendGridClient(emailApiKey);
            sender = new EmailAddress("captain.nemo.github@gmail.com", "Captain Nemo");
        }
        #endregion Private Functions and Initialiser

        #region Public Functions
        // public static string SendWhatsAppMessage(string pushMessage)
        // {
        //     string response = "";
        //     TwilioClient.Init(Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID"),
        //                         Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN")
        //                     );
        //     try
        //     {
        //         var message = MessageResource.Create(
        //                             from: new PhoneNumber("whatsapp:+14155238886")// +18302613236
        //                             , to: new PhoneNumber("whatsapp:+918527508989")
        //                             , body: pushMessage
        //                         // ,mediaUrl: new List<Uri>{new Uri(imageUrl)}
        //                         );
        //         response = "Message SID: " + message.Sid;
        //     }
        //     catch(Exception ex)
        //     {
        //         response = ex.Message.ToString();
        //     }
        //     return response;
        // }
        public static async Task<string> RegisterEmailAsync(Registration user)
        {
            string htmlContent = $"Hi {user.Name}!\n"
            +$"Your details have been registered with us.\n"
            +$"You will recieve notifications on <strong>{user.EmailID}</strong>";
            

            return await SendEmail(userName: user.Name,
                        userEmail: user.EmailID,
                        htmlContent: htmlContent
                        );
        }
        public static async Task<string> SendEmail(string userName, string userEmail, string htmlContent, string plainContent = "")
        {            
            // EmailAddress to = new EmailAddress("rajchaks1969@gmail.com", "Papaila");
            // string plainTextContent = "Trial email! Hello World!";
            // string htmlContent = "<strong>Coding is easy to do anywhere!</strong>";
            EmailAddress reciever = StructureEmailID(userName, userEmail);
            SendGridMessage msg = MailHelper.CreateSingleEmail(sender, reciever, EMAIL_SUBJECT, plainContent, htmlContent);
            Response response = await client.SendEmailAsync(msg);
            
            string responseMessage = (response.IsSuccessStatusCode)?
                                                        ("Email Sent Succesfully"):
                                                        (await response.Body.ReadAsStringAsync());
            return responseMessage;
        }
        #endregion Public Functions
    }
}