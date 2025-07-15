using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using static Api.Logger.Logger;

namespace Api.CommonFuncations
{
    public class Email
    {
       

        public static void SendEmail(string email, string subject,string emails_r, string sender_email, string sender_name, string sender_password, int port, string host, ref List<LogLines> lines)
        {
            MailMessage msg = new MailMessage();
            if (emails_r.Contains(";"))
            {
                string[] email_receipent = emails_r.Split(';');
                foreach(string e in email_receipent)
                {
                    msg.To.Add(new MailAddress(e, e));
                }
            }
            else
            {
                msg.To.Add(new MailAddress(emails_r, emails_r));
            }
            

            //msg.From = new MailAddress("yellowbetnotifications@gmail.com", "Yellowbet Notifications");
            msg.From = new MailAddress(sender_email, sender_name);
            msg.Subject = subject;
            string body = "";
            try
            {
                body = System.IO.File.ReadAllText(@"E:\apps\iDoBetPayout\NewCustomerP1.html");
            }
            catch
            {
                try
                {
                    body = System.IO.File.ReadAllText(@"d:\apps\iDoBetPayout\NewCustomerP1.html");
                }
                catch
                {

                }
            }
            

            body = body + email;


            body = body + System.IO.File.ReadAllText(@"E:\apps\iDoBetPayout\NewCustomerP2.html");

            msg.Body = body;
            msg.IsBodyHtml = true;
            




            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;

            //client.Credentials = new System.Net.NetworkCredential("yellowbetnotifications@gmail.com", "EdPGKPUkjiqepiD18T9T");
            client.Credentials = new System.Net.NetworkCredential(sender_email, sender_password);
            client.Port = port; // 587; // You can use Port 25 if 587 is blocked (mine is!)
            client.Host = host; // "smtp.gmail.com";

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            try
            {
                client.Send(msg);
                lines = Add2Log(lines, "E-Mail Sent Succesfully", 100, "reg_interworks");

            }
            catch (Exception ex)
            {
                lines = Add2Log(lines, "E-Mail Failed" + ex.ToString(), 100, "reg_interworks");

            }
        }
    }
}