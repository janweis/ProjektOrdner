using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using ProjektOrdner.App;
using ProjektOrdner.Repository;
using ProjektOrdner.Permission;

namespace ProjektOrdner.Mail
{
    public class MailProcessor
    {
        private AppSettings AppSettings { get; set; }
        private RepositoryFolder ProjektOrdner { get; set; }

        public MailProcessor(AppSettings appSettings, RepositoryFolder projektOrdner)
        {
            AppSettings = appSettings;
            ProjektOrdner = projektOrdner;
        }


        /// <summary>
        /// 
        /// Erstellt Mail anhand von Templates
        /// 
        /// </summary>
        public MimeMessage CreateMail(string mailtext, string betreff, AdUser user)
        {
            MimeMessage mail = new MimeMessage();
            mail.From.Add(new MailboxAddress(AppConstants.MailFromAddress, AppConstants.MailFromAddress));
            mail.To.Add(new MailboxAddress($"{user.Vorname} {user.Nachname}", user.Email));
            mail.Subject = betreff;
            mail.Body = new TextPart("plain")
            {
                Text = mailtext
            };

            return mail;
        }

        /// <summary>
        /// 
        /// Sendet Mails
        /// 
        /// </summary>
        public void SendMail(MimeMessage mail)
        {
            using (SmtpClient client = new SmtpClient())
            {
                client.Connect(AppConstants.MailServer, AppConstants.MailPort, AppConstants.MailUsingSSL);

                // Note: only needed if the SMTP server requires authentication
                //client.Authenticate("joey", "password");

                client.Send(mail);
                client.Disconnect(true);
            }
        }
    }
}
