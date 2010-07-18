using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace IronAHK.Rusty
{
    partial class Core
    {
        /// <summary>
        /// Resolves a host name or IP address.
        /// </summary>
        /// <param name="name">The host name.</param>
        /// <returns>A dictionary with the following key/value pairs:
        /// <list type="bullet">
        /// <item><term>Host</term>: <description>the host name.</description></item>
        /// <item><term>Addresses</term>: <description>the list of IP addresses.</description></item>
        /// </list>
        /// </returns>
        public static Dictionary<string, object> GetHostEntry(string name)
        {
            var entry = Dns.GetHostEntry(name);
            var ips = new string[entry.AddressList.Length];

            for (int i = 0; i < ips.Length; i++)
                ips[i] = entry.AddressList[0].ToString();

            var info = new Dictionary<string, object>();
            info.Add("Host", entry.HostName);
            info.Add("Addresses", ips);
            return info;
        }

        /// <summary>
        /// Sends an email.
        /// </summary>
        /// <param name="recipients">A list of receivers of the message.</param>
        /// <param name="subject">Subject of the message.</param>
        /// <param name="message">Message body.</param>
        /// <param name="options">A dictionary with any the following optional key/value pairs:
        /// <list type="bullet">
        /// <item><term>Attachments</term>: <description>a list of file paths to send as attachments.</description></item>
        /// <item><term>Bcc</term>: <description>a list of blind carbon copy recipients.</description></item>
        /// <item><term>CC</term>: <description>a list of carbon copy recipients.</description></item>
        /// <item><term>From</term>: <description>the from address.</description></item>
        /// <item><term>ReplyTo</term>: <description>the reply address.</description></item>
        /// <item><term>Host</term>: <description>the SMTP client hostname and port.</description></item>
        /// <item><term>(Header)</term>: <description>any additional header and corresponding value.</description></item>
        /// </list>
        /// </param>
        /// <remarks><see cref="ErrorLevel"/> is set to <c>1</c> if there was a problem, <c>0</c> otherwise.</remarks>
        public static void Mail(object recipients, string subject, string message, IDictionary options = null)
        {
            ErrorLevel = 1;

            var msg = new MailMessage { Subject = subject, Body = message };
            msg.From = new MailAddress(string.Concat(Environment.UserName, "@", Environment.UserDomainName));

            if (recipients is string)
                msg.To.Add(new MailAddress((string)recipients));
            else if (recipients is IEnumerable)
            {
                foreach (var item in (IEnumerable)recipients)
                    if (!string.IsNullOrEmpty(item as string))
                        msg.To.Add((string)item);
            }
            else
                return;

            var smtpHost = "localhost";
            int? smtpPort = null;

            if (options == null)
                goto send;

            #region Options

            foreach (var key in options.Keys)
            {
                var item = key as string;

                if (string.IsNullOrEmpty(item))
                    continue;

                string[] value;

                if (options[key] is string)
                    value = new[] { (string)options[key] };
                else if (options[key] is string[])
                    value = (string[])options[key];
                else if (options[key] is object[])
                {
                    var block = (object[])options[key];
                    value = new string[block.Length];

                    for (int i = 0; i < block.Length; i++)
                        value[i] = block[i] as string;
                }
                else
                    continue;

                switch (item.ToLowerInvariant())
                {
                    case Keyword_Attachments:
                        foreach (var entry in value)
                            if (File.Exists(entry))
                                msg.Attachments.Add(new Attachment(entry));
                        break;

                    case Keyword_Bcc:
                        foreach (var entry in value)
                            msg.Bcc.Add(entry);
                        break;

                    case Keyword_CC:
                        foreach (var entry in value)
                            msg.CC.Add(entry);
                        break;

                    case Keyword_From:
                        msg.From = new MailAddress(value[0]);
                        break;

                    case Keyword_ReplyTo:
                        msg.ReplyTo = new MailAddress(value[0]);
                        break;

                    case Keyword_Host:
                        {
                            smtpHost = value[0];

                            var z = smtpHost.LastIndexOf(Keyword_Port);

                            if (z != -1)
                            {
                                var port = smtpHost.Substring(z + 1);
                                smtpHost = smtpHost.Substring(0, z);
                                int n;

                                if (int.TryParse(port, out n))
                                    smtpPort = n;
                            }
                        }
                        break;

                    default:
                        msg.Headers.Add(item, value[0]);
                        break;
                }
            }

            #endregion

        send:
            var client = smtpPort == null ? new SmtpClient(smtpHost) : new SmtpClient(smtpHost, (int)smtpPort);

            try
            {
                client.Send(msg);
                ErrorLevel = 0;
            }
            catch (SmtpException)
            {
                if (Debug)
                    throw;
            }
        }
    }
}
