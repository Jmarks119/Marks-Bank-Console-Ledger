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
        private static SmtpClient client;

        static EmailService()
        {
            client = new SmtpClient
            {
                Host = "smtp.mail.yahoo.com",
                Port = 465,
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("marksbankledger@yahoo.com", "ThisIsABadPractice9"), //Obviously network credentials should never be in code that's checked into version control.
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000
            };
        }

        internal static bool SendLoginEmail(MailAddress email, string confirmationString)
        {
            throw new NotImplementedException();
        }
    }
}
