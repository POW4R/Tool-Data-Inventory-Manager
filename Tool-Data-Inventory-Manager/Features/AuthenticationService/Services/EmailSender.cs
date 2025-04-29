using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tool_Data_Inventory_Manager
{
    public static class EmailSender
    {
        public static void SendTemporaryCode(string toEmail, string code)
        {
            var fromAddress = new MailAddress("tool.data.inventory.manager@gmail.com", "Tool-Data-Inventory-Manager");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "lewe sbjq dnik lmmq";

            const string subject = "Elfelejtett jelszó - Ideiglenes kód";
            string body = $"Az ideiglenes kódod: {code}\nEz a kód 10 percig érvényes.";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                smtp.Send(message);
            }
        }
    }
}
