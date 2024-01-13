namespace SciChatProject.Models
{
    public class Message : SQLClass
    {
        public static string? TableName { get; set; } = "messagestable";

        public int id { get; set; }

        [SQLProperty(Name="content")] public string Content { get; set; }
        [SQLProperty(Name="userid")] public int UserID { get; set; }
        [SQLProperty(Name="conversationid")] public int ConversationID { get; set; }

        public static void SendMessage(string content, int userid, int conversationid)
        {
            var msg = new Message { Content=content, UserID = userid, ConversationID = conversationid };
            SendMessage(msg);
        }
        private static void SendMessage(Message message)
        {
            DataBaseHelper.ExecuteChange("messagestable", new List<Message> { message}, DataBaseHelper.ChangeType.Insert);
        }

        public User GetUser()
        {
            return DataBaseHelper.GetObjects<User>().First(x => x.id==UserID); 
        }
        public Conversation GetConversation()
        {
            return DataBaseHelper.GetObjects<Conversation>().First(x => x.id == ConversationID);
        }
    }
}
