using System;
using System.Net.Mail;
using CloudInit.Notification.Core;
using Microsoft.Win32;
using System.Net;

namespace CloudInit.Notification
{
    /// <summary>
    /// Sends the notification via email
    /// </summary>
    public class EmailNotificationProvider : NotificationProviderBase
    {
        /// <summary>
        /// Gets the name of the provider
        /// </summary>
        public override string ProviderName
        {
            get { return "EmailNotification"; }
        }
        /// <summary>
        /// Adds this provider to the registry
        /// </summary>
        public override void Add()
        {
            base.Add();

            this.SetSetting("ToEmail", String.Empty, RegistryValueKind.String);
            this.SetSetting("FromEmail", String.Empty, RegistryValueKind.String);
            this.SetSetting("MailServer", String.Empty, RegistryValueKind.String);
        }
        /// <summary>
        /// Notifies the provider of a CloudInit log file
        /// </summary>
        /// <param name="session">The current CloudInit session</param>
        public override void Notify(CloudInitSession session)
        {
            // Get the settings from the registry
            String toEmail = this.GetSetting<String>("ToEmail");
            String fromEmail = this.GetSetting<String>("FromEmail");
            String mailServer = this.GetSetting<String>("MailServer");
            String username = this.GetSetting<String>("Username");
            String password = this.GetSetting<String>("Password");
            Boolean integratedAuthentication = this.GetSetting<Boolean>("IntegratedAuthentication");
            Boolean enableSsl = this.GetSetting<Boolean>("EnableSSL");

            // Create a message we want to send
            MailMessage message = new MailMessage(fromEmail, toEmail);
            message.Subject = String.Format("LOG Notification - instance:{0} {1}", session.Machine, session.Address);
            message.Body = session.Data;

            // Send the message
            SmtpClient smtp = new SmtpClient(mailServer);

            // If the user has integrated authentication turned on assign the default network credentials, 
            // if they provided a username and password then use that instead.
            if (integratedAuthentication)
                smtp.Credentials = CredentialCache.DefaultNetworkCredentials;
            else if(!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(password))
                smtp.Credentials = new NetworkCredential(username, password);

            // Enable SSL if the user requests this
            if (enableSsl)
                smtp.EnableSsl = true;

            smtp.Send(message);
        }
    }
}
