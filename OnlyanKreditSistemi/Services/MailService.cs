using OnlyanKreditSistemi.Services.Interfaces;
using System.Net.Mail;
using System.Net;

namespace OnlyanKreditSistemi.Services
{
    public class MailService : IMailService
    {
        public async Task SendMail(string from, string to, string subject, string link)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(from);
            mailMessage.To.Add(to);
            mailMessage.Subject = subject;
            mailMessage.Body = $"Your verify email link: ${link}";

            SmtpClient smtpClient = new SmtpClient();
            smtpClient.Host = "smtp.gmail.com";
            smtpClient.Port = 587;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(from, "xdwvefqanrjnagxr");
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("Email Sent Successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
