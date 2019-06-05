using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Mimir.Util
{
    public class MailWorker
    {
        public static string Send(string recipient, string title, string content, bool isHtml = false)
        {
            try
            {
                MailMessage mail = new MailMessage(new MailAddress(Program.SmtpEmail, Program.SmtpName), new MailAddress(recipient));
                mail.Subject = title;
                mail.IsBodyHtml = isHtml;
                mail.Body = content;
                SmtpClient smtp = new SmtpClient(Program.SmtpDomain, Program.SmtpPort);
                smtp.EnableSsl = Program.SmtpIsSsl;
                smtp.Credentials = new NetworkCredential(Program.SmtpEmail, Program.SmtpPassword);
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return string.Empty;
        }
    }
}
