using Demo.DAL.Entities;
using System.Net;
using System.Net.Mail;

namespace Demo.PL.Helper
{
    public class EmailSettings
    {
        public static void SendEmail(Email email)
        {
            var client = new SmtpClient("smtp.ethereal.email", 587);//server send to mail
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("alvina.goldner@ethereal.email", "76evXz9grn63BjGPJ1");
            client.Send("Monica@gmail.com", email.To, email.Title, email.Body);
        }
    }
}
