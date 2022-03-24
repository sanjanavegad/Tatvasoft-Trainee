using Helperland.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Helperland.Core
{
    public class MailHelper
    {
        private readonly IConfiguration _configuration;

        public MailHelper(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public bool Send(EmailModel model)
        {
            try
            {
                var host = _configuration["Gmail:Host"];
                var port = int.Parse(_configuration["Gmail:Port"]);
                var username = _configuration["Gmail:Username"];
                var password = _configuration["Gmail:Password"];
                var enable = bool.Parse(_configuration["Gmail:SMTP:starttls:enable"]);
                model.From = _configuration["Gmail:adminemail"];
                var smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = enable,
                    Credentials = new NetworkCredential(username, password)
                };
                var mailMessage = new MailMessage(model.From, model.To, model.Subject, model.Body);
                mailMessage.IsBodyHtml = true;
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SendContactUs(EmailModel model)
        {
            try
            {
                var host = _configuration["Gmail:Host"];
                var port = int.Parse(_configuration["Gmail:Port"]);
                var username = _configuration["Gmail:Username"];
                var password = _configuration["Gmail:Password"];
                var enable = bool.Parse(_configuration["Gmail:SMTP:starttls:enable"]);
                model.From = _configuration["Gmail:Username"];
                model.To = _configuration["Gmail:adminemail"];
                var smtpClient = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = enable,
                    Credentials = new NetworkCredential(username, password)
                };
                var mailMessage = new MailMessage(model.From, model.To, model.Subject, model.Body);
                mailMessage.IsBodyHtml = true;
                if (model.Attachment != null)
                {
                    mailMessage.Attachments.Add(new Attachment(model.Attachment));
                }
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
