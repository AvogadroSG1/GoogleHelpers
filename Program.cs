using System;

using System.Text;
using System.Threading.Tasks;

namespace Peter.Google.Gmail
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GmailSupport gmailSupport = new GmailSupport();
            gmailSupport.SetMessage(args[0], args[1], args[2]);
            await gmailSupport.SendMail();
        }
    }
}
