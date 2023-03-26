using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace SShop.Services.MailJet
{
    public class MailJetServices : IMailJetServices
    {
        private readonly string MJ_APIKEY_PUBLIC = "5469fcd0f7928e3e7e6a30893195bc43";
        private readonly string MJ_APIKEY_PRIVATE = "fd4a44689e032f7633e7dc6f0f31d059";

        public async Task<bool> SendMail(string name, string email, string content, string title)
        {
            try
            {
                MailjetClient client = new MailjetClient(MJ_APIKEY_PUBLIC, MJ_APIKEY_PRIVATE);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                   .Property(Send.FromEmail, "nguyenminhson102002@gmail.com")
                   .Property(Send.FromName, "FurSshop")
                   .Property(Send.Subject, title)
                   .Property(Send.HtmlPart, content)
                   .Property(Send.Recipients, new JArray {
                            new JObject {
                                 {"Email", email},
                                 {"Name", name}
                            }
                       });
                MailjetResponse response = await client.PostAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}