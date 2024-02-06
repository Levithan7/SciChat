namespace SciChatProject.Models
{
    public class User : SQLClass
    {
        public static string? TableName { get; set; } = "users";
        public int id { get; set; }
        [SQLProperty(Name="username")] public string? UsernameTEST { get; set; }

        public List<Message> GetMessages()
        {
            return DataBaseHelper.GetObjects<Message>().Where(x=>x.UserID==id).ToList();
        }

        public List<Conversation> GetConversations()
        {
            return DataBaseHelper.GetObjects<Conversation>().Where(x => x.GetUsers().Any(x => x.id == id)).ToList();
        }

        public void PutUserInDataBase()
        {
            DataBaseHelper.ExecuteChange(TableName, new List<User> { this }, DataBaseHelper.ChangeType.Insert);
        }

        public static User GetUserByID(int id)
        {
            return DataBaseHelper.GetObjects<User>().First(x => x.id == id);
        }
    }
}
