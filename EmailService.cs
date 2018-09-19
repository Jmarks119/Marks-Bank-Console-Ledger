using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MarksBankLedger
{
    internal static class EmailService
    {
        private static readonly SmtpClient client;

        static EmailService()
        {
            client = new SmtpClient
            {
                Host = "smtp.mail.yahoo.com",
                Port = 587,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("marksbankledger@yahoo.com", "ThisIsABadPractice9"), //Obviously credentials should never be in code that's checked into version control.
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000
            };
        }

        internal static bool SendLoginEmail(MailAddress recieveAddress, string confirmationString)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("marksbankledger@yahoo.com");
                mail.To.Add(recieveAddress);
                mail.Subject = "Your Login For MarksBank";
                mail.Body = "Your login token for Marks Bank is " + confirmationString;
                client.Send(mail);
                return true;
            }
            catch (SmtpException)
            {
                return false;
            }
        }
    }
}
