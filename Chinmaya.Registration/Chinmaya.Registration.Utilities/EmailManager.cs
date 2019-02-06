using Chinmaya.Registration.Utilities;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Chinmaya.Utilities
{
    /// <summary>
    /// Sends email using smtp
    /// </summary>
    public class EmailManager
    {
        #region Declaration of variables
        private readonly MailMessage _mail;
        private String _To;
        private String _CC;
        #endregion

        public EmailManager()
        {
            _mail = new MailMessage();
            _mail.Sender = _mail.From = new MailAddress(ConfigurationManager.AppSettings["SMTPUsername"]);
            _mail.IsBodyHtml = true;
        }

        public int Id { get; set; }

        public bool IsHtml
        {
            set { _mail.IsBodyHtml = value; }
        }

        public string To
        {
            set { _To = value; }
        }

        public string From
        {
            set { _mail.Sender = _mail.From = new MailAddress(value); }
        }

        public string CC
        {
            set { _CC = value; }
        }

        public string Subject
        {
            set { _mail.Subject = value; }
        }

        public string Body
        {
            get { return Body; }
            set { _mail.Body = value; }
        }
		
		public void Send()
		{
			var from = new EmailAddress("testsmtp@cesltd.com");
			var subject = _mail.Subject;
			var to = new EmailAddress(_To);
			var plainTextContent = _mail.Body;
			var htmlContent = "";
			var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
			Execute(msg).Wait(1200);
		}

		static async Task Execute(SendGridMessage message)
		{
				var apiKey = ConfigurationManager.AppSettings["SendGridKey"].ToString().Trim();
				var client = new SendGridClient(apiKey);
				var response = await client.SendEmailAsync(message);
		}
	}
}
