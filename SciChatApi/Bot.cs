using SciChatProject;
using SciChatProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace SciChatApi
{
    internal class Bot
    {
        private static string? TableName { get; set; } = "users";
        public int id { get; set; }
        [SQLProperty(Name = "username")] public string? BotName { get; set; }

        private List<Message> ReceivedMessages { get; set; } = new List<Message>();
        private static HttpClient client = new HttpClient();
        private static string serverurl = $"{Constants.APIURL}/api/Server";
        
        public List<Message> GetSentMessages()
        {
            return JsonSerializer.Deserialize<List<Message>>(FetchData($"{serverurl}/convsByUserID&userid={id}"));
        }

        public List<Message> GetReceivedmessages(bool update = false)
        {
            if(update)
                UpdateReceivedMessages();
            return ReceivedMessages;
        }

        public List<Conversation>? GetConversations()
        {
            return JsonSerializer.Deserialize<List<Conversation>>(FetchData($"{serverurl}/convsByUserID?userid={id}"));
        }

        public void SendMessage(string content, int convID)
        {
            var parameters = new Dictionary<string, string>
            {
                {"userid", id.ToString()  },
                {"conversationid", convID.ToString() },
                {"content", content}
            };
            PostData($"{serverurl}/addmessage", parameters);
        }

        public List<Message> UpdateReceivedMessages()
        {
            var allMessages = JsonSerializer.Deserialize<List<Message>>(FetchData($"{serverurl}/userReceivedMessagesByID?userid={id}&includeOwn=false"));
            var newMessages = allMessages.Where(x => !ReceivedMessages.Select(y=>y.id).Contains(x.id)).ToList();
            ReceivedMessages = allMessages;
            return newMessages;
        }

        private static string FetchData(string request)
        {
            var task = client.GetAsync(request);
            task.Wait();
            var reader = new StreamReader(task.Result.Content.ReadAsStream());
            return reader.ReadToEnd();
        }

        private static void PostData(string url, Dictionary<string, string> parameters)
        {
            //var content = new FormUrlEncodedContent(parameters);
            //var json = JsonSerializer.Serialize(parameters);
            string queryString = string.Join("&", parameters.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

            string fullUrl = $"{url}?{queryString}";

            var task = client.PostAsync(fullUrl, null);
            task.Wait();
        }
    }
}