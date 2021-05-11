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
using System.Collections.Generic;
using System.Text;

namespace CoWinAlert.Utils
{
    public static class Notifications
    {
        #region Private Members
        private static SendGridClient client;
        private static EmailAddress sender;
        private static string EMAIL_SUBJECT = "CoWin Alert";
        private static string CENTER_DETAILS = "<p><strong>Center ID:</strong> {0}<br>"
                                            +"<strong>Name:</strong> {1}<br>"
                                            +"<strong>Address:</strong> {2}<br>"
                                            +"<strong>State:</strong> {3}<br>"
                                            +"<strong>District:</strong> {4}<br>"
                                            +"<strong>Pincode:</strong> {5}<br><p>";
        private static string TIME_DETAILS = "<p>"//<strong>Date:</strong> {0}<br>"
                                            +"<strong>From:</strong> {1}        "
                                            +"<strong>To:</strong> {2}<br><p>";
        private static string FEE_DETAILS = "<p><ul><li><strong>Vaccine:</strong> {0}<br></li>"
                                            +"<li><strong>Fees:</strong> {1}<br></li></ul><p>";
        private static string TABLE_HEADER = "<table style=\"width:50%\"><tr>"
                                            +"<th>SessionDate</th>"
                                            +"<th>Min_age_limit</th>"
                                            +"<th>Vaccine</th>"
                                            +"<th>Available_capacity</th>";
                                            // +"<th>Slots</th></tr>";
        private static string ROW_DETAILS = "<tr>"
                                            +"<td>{0}</td>"
                                            +"<td>{1}</td>"
                                            +"<td>{2}</td>"
                                            +"<td>{3}</td>"
                                            // +"<td>{4}</td>"
                                            +"</tr>";
        private static string TABLE_BREAK = "<tr><td></td></tr>"+
                                            "<tr><td></td></tr>"+
                                            "<tr><td></td></tr>"+
                                            "<tr><td></td></tr>";
        private static string TABLE_END = "</table><p><hr><p>";
        

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
        public static async Task<string> RegisterEmailAsync(RegistrationDTO user)
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
            string responseMessage = "Email Sent Succesfully";
            EmailAddress reciever = StructureEmailID(userName, userEmail);
            SendGridMessage msg = MailHelper.CreateSingleEmail(sender, reciever, EMAIL_SUBJECT, plainContent, htmlContent);
            Response response = await client.SendEmailAsync(msg);
            
            responseMessage = (response.IsSuccessStatusCode)?
                                                        responseMessage:
                                                        (await response.Body.ReadAsStringAsync());
            return responseMessage;
        }
        public static string StructureSessionEmailBody(IEnumerable<SessionCalendarDTO> input)
        {
            StringBuilder emailBody = new StringBuilder();
            foreach(SessionCalendarDTO center in input)
            {
                emailBody.AppendFormat(CENTER_DETAILS
                                ,center.Center_id
                                ,center.Name
                                ,center.Address
                                ,center.State_name
                                ,center.District_name
                                ,center.Pincode
                            );

                emailBody.AppendFormat(TIME_DETAILS
                                ,center.From.ToLongDateString()
                                ,center.From.ToShortTimeString()
                                ,center.To.ToShortTimeString()
                            );
                
                if(center.Fee_type != FeeTypeDTO.FREE && center.Vaccine_fees != null)
                {
                    foreach(VaccineFeeDTO vaccineFee in center.Vaccine_fees)
                    {
                        emailBody.AppendFormat(FEE_DETAILS
                                        ,vaccineFee.vaccine
                                        ,vaccineFee.fee
                                    );
                    }                    
                }
                
                emailBody.Append(TABLE_HEADER);
                
                foreach(SessionDTO session in center.Sessions)
                {
                    emailBody.AppendFormat(ROW_DETAILS
                                    ,session.SessionDate.ToString("dd\\-MM\\-yyyy")
                                    ,session.Min_age_limit.ToString()
                                    ,session.Vaccine.ToString()
                                    ,session.Available_capacity.ToString()
                                    // ,slots
                                );
                    // foreach(string slots in session.Slots)
                    // {
                    //     emailBody.AppendFormat(ROW_DETAILS
                    //                 ,session.SessionDate.ToString("dd\\-MM\\-yyyy")
                    //                 ,session.Min_age_limit.ToString()
                    //                 ,session.Vaccine.ToString()
                    //                 ,session.Available_capacity.ToString()
                    //                 ,slots
                    //             );
                    // }
                    emailBody.Append(TABLE_BREAK);
                }
                emailBody.Append(TABLE_END);
            }
            return emailBody.ToString();
        }
        #endregion Public Functions
    }
}