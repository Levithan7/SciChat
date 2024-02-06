namespace SciChatProject.Models
{
    public class Conversation : SQLClass
    {
        public static string? TableName { get; set; } = "conversations";
        public int id {  get; set; }
        [SQLProperty(Name="conversationname")] public string ConversationName { get; set; }

        public List<Message> GetMessages()
        {
            return DataBaseHelper.GetObjects<Message>().Where(x => x.ConversationID==id).Select(x => new Message { Content = x.Content }).ToList();
        }

        public List<User> GetUsers()
        {
            return DataBaseHelper.GetObjects<UserConversationLink>().Where(x=>x.ConversationID==id).Select(x=>new User { id=x.UserID }).ToList();
        }


        public List<User> GetUser(string m)
		{
			return DataBaseHelper.GetObjects<Message>().Where(x => x.ConversationID == id).Where(x => x.Content == m).Select(x => new User { id = x.UserID }).ToList();
		}

		public List<User> GetUsersName(int uid)
		{
			return DataBaseHelper.GetObjects<User>().Where(x => x.id == uid).Select(x => new User { UsernameTEST = x.UsernameTEST }).ToList();
		}

		public void AddUserToConversation(User user)
        {
            UserConversationLink link = new UserConversationLink { UserID = user.id, ConversationID = id };
            DataBaseHelper.ExecuteChange(UserConversationLink.TableName, new List<UserConversationLink>{link}, DataBaseHelper.ChangeType.Insert);
        }

        public static Conversation GetConversationByID(int id)
        {
            return DataBaseHelper.GetObjects<Conversation>().First(x=>x.id==id);
        }
	}
}