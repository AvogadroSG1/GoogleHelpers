using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Peter.Google.Gmail
{
    public class GmailSupport
    {
        private string ApplicationName = "Gmail API .NET Quickstart";
        private string[] Scopes = { GmailService.Scope.GmailCompose, GmailService.Scope.MailGoogleCom };
        private readonly GmailService gmailService;
        private GoogleMailMessage googleMailMessage;
        private readonly UserCredential credential;

        public GmailSupport()
        {
            this.credential = this.GetUserPermissions();
            this.gmailService = this.CreateGmailClient();
        }

        public GmailSupport(GoogleMailMessage message)
        {
            this.googleMailMessage = message;
            this.credential = this.GetUserPermissions();
            this.gmailService = this.CreateGmailClient();
        }

        private UserCredential GetUserPermissions()
        {
            UserCredential credential;
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            return credential;
        }

        private GmailService CreateGmailClient()
        {
            return new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }
        public void SetMessage(string emailAddress, string subject, string body)
        {
            this.googleMailMessage = new GoogleMailMessage()
            {
                EmailAddress = emailAddress,
                Subject = subject,
                Body = body
            };
        }

        public void SetMessage(GoogleMailMessage message)
        {
            this.googleMailMessage = message;
        }

        public async Task SendMail()
        {
            UsersResource.MessagesResource.SendRequest response = gmailService.Users.Messages.Send(this.googleMailMessage.GmailMessage, "me");

            Message test = await response.ExecuteAsync();
            Console.WriteLine(test.ToString());
        }
    }

    public class GoogleMailMessage
    {
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public Message GmailMessage
        {
            get
            {
                Message m = new Message();
                m.Raw = $"From: {this.EmailAddress}\r\nTo: {this.EmailAddress}\r\nSubject: {this.Subject}\r\n\r\n{this.Body}".EncodeBase64();
                return m;
            }
        }
    }


    public static class ExtensionMethods
    {
        public static string EncodeBase64(this string value)
        {
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        public static string DecodeBase64(this string value)
        {
            byte[] valueBytes = System.Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }
    }
}