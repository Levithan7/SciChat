using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using SciChatApi.Models;

namespace SciChatApi
{
    internal class Bot : User
    {
        private List<Message> ReceivedMessages { get; set; } = new List<Message>();
        private static HttpClient client = new HttpClient();
        private static string serverurl = $"{Constants.APIURL}/api/Server";
      

        // TODO
        public User? GetUserById()
        {
            var param = new Dictionary<string, string>();
            return JsonSerializer.Deserialize<User>(this.FetchViaParam("", param));
        }

        public List<Message> GetSentMessages()
        {
            var param = new Dictionary<string, string>() { { "userid", $"{id}"} };
            return JsonSerializer.Deserialize<List<Message>>(this.FetchViaParam("convsByUserID", param)) ?? new List<Message>();
        }

        public List<Message> GetReceivedmessages(bool update = false)
        {
            if(update)
                UpdateReceivedMessages();
            return ReceivedMessages;
        }

        public List<Conversation> GetConversations()
        {
            var param = new Dictionary<string, string>() { { "userid", $"{id}" } };
            return JsonSerializer.Deserialize<List<Conversation>>(this.FetchViaParam("convsByUserID", param)) ?? new List<Conversation>();
        }

        public void SendMessage(string content, int convID)
        {
            var parameters = new Dictionary<string, string>
            {
                {"userid", id.ToString()  },
                {"conversationid", convID.ToString() },
                {"content", content}
            };
            this.PostData($"{serverurl}/addmessage", parameters);
        }

        public List<Message> UpdateReceivedMessages()
        {
            var param = new Dictionary<string, string> { { "userid", $"{id}" }, { "includeOwn", "false" } };
            var allMessages = JsonSerializer.Deserialize<List<Message>>(this.FetchViaParam("userReceivedMessagesByID", param)) ?? new List<Message>();
            var newMessages = allMessages.Where(x => !ReceivedMessages.Select(y=>y.id).Contains(x.id)).ToList();
            ReceivedMessages = allMessages;
            return newMessages;
        }

        private string FetchViaParam(string suburl, Dictionary<string, string> parameters, bool addCredentials = true)
        {
			parameters = AddCredentials(parameters, addCredentials);
			return FetchViaParam(suburl, parameters);
        }

        private static string FetchViaParam(string suburl, Dictionary<string, string> parameters)
        {
            suburl.TrimStart('/');
			string queryString = $"{serverurl}/{suburl}?" + string.Join("&", parameters.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            return FetchData(queryString);
		}

        private static string FetchData(string request)
        {
            var task = client.GetAsync(request);
            task.Wait();
            var reader = new StreamReader(task.Result.Content.ReadAsStream());
            return reader.ReadToEnd();
        }

        private void PostData(string url, Dictionary<string, string> parameters, bool addCredentials = true)
        {
            parameters = AddCredentials(parameters, addCredentials);
            PostData(url, parameters);
        }

        private static void PostData(string url, Dictionary<string, string> parameters)
        {
            string queryString = string.Join("&", parameters.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

            string fullUrl = $"{url}?{queryString}";

            var task = client.PostAsync(fullUrl, null);
            task.Wait();
        }

        private Dictionary<string, string> AddCredentials(Dictionary<string, string> param, bool addCredentials)
        {
			if (addCredentials)
			{
				param.Add("u", $"{id}");
				param.Add("p", Password);
			}
            return param;
		}
    }
}