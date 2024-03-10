namespace SciChatProject.Models
{
    public class Conversation : SQLClass
    {
        public static string? TableName { get; set; } = "conversations";
        public int id {  get; set; }
        [SQLProperty(Name="conversationname")] public string ConversationName { get; set; }

        public List<Message> GetMessages()
        {
            return DataBaseHelper.GetObjects<Message>().Where(x => x.ConversationID==id).OrderByDescending(x => x.id).ToList();
        }

        public List<User> GetUsers()
        {
            return DataBaseHelper.GetObjects<UserConversationLink>().Where(x=>x.ConversationID==id).Select(x=>User.GetUserByID(x.UserID)).ToList();
        }

		public void AddUserToConversation(User user)
        {
            UserConversationLink link = new UserConversationLink { UserID = user.id, ConversationID = id };
            DataBaseHelper.ExecuteChange(UserConversationLink.TableName, new List<UserConversationLink>{link}, DataBaseHelper.ChangeType.Insert);
        }

        public static void AddUserToNewConversation(User user1, User user2, string name)
        {
            DataBaseHelper.ExecuteChange(TableName, new List<Conversation> { new Models.Conversation { ConversationName = name} }, DataBaseHelper.ChangeType.Insert);
            DataBaseHelper.GetObjects<Conversation>().Last().AddUserToConversation(user1);
            DataBaseHelper.GetObjects<Conversation>().Last().AddUserToConversation(user2);
        }
        
        public static Conversation GetConversationByID(int id)
        {
            return DataBaseHelper.GetObjects<Conversation>().First(x=>x.id==id);
        }
	}
}