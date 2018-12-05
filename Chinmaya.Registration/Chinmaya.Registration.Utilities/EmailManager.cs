using Chinmaya.Registration.Utilities;
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
            EncryptDecrypt objEncryptionAlgorithm = new EncryptDecrypt();
            var sc = new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["SMTPAddress"],
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = false,
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["SMTPPort"]),
                Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SMTPUsername"], ConfigurationManager.AppSettings["SMTPPassword"])
            };

            if (!string.IsNullOrEmpty(_To))
            {
                if (_To.Contains(","))
                {
                    string[] userInfo = _To.Split(',');
                    foreach (string to in userInfo)
                    {
                        _mail.To.Add(new MailAddress(to));
                    }
                }
                else
                {
                    _mail.To.Add(new MailAddress(_To));
                }
            }

            if (!string.IsNullOrEmpty(_CC))
            {
                if (_CC.Contains(","))
                {
                    string[] userInfo = _CC.Split(',');
                    foreach (string cc in userInfo)
                    {
                        _mail.CC.Add(new MailAddress(cc));
                    }
                }
                else
                {
                    _mail.CC.Add(new MailAddress(_CC));
                }
            }

            sc.Send(_mail);
        }
    }
}
