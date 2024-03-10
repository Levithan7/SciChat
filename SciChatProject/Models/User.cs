namespace SciChatProject.Models
{
    public class User : SQLClass
    {
        public new static string TableName { get; set; } = "users";
        public int id { get; set; }
        [SQLProperty(Name ="password")] public string? Password { get; set; }
        [SQLProperty(Name="username")] public string? Username { get; set; }

        public List<Message> GetMessages()
        {
            return DataBaseHelper.GetObjects<Message>().Where(x=>x.UserID==id).ToList();
        }

        public List<Conversation> GetConversations()
        {
            return DataBaseHelper.GetObjects<Conversation>().Where(x => x.GetUsers().Any(x => x.id == id)).ToList();
        }

        public static void PutUserInDataBase(string pass, string u)
        {
            DataBaseHelper.ExecuteChange(TableName, new List<User> { new Models.User { Password = pass, Username = u } }, DataBaseHelper.ChangeType.Insert);
        }

        public static User? GetUserByID(int id)
        {
            return DataBaseHelper.GetObjects<User>()?.FirstOrDefault(x => x.id == id);
        }

        public static User GetLastUser()
        {
            return DataBaseHelper.GetObjects<User>().Last();
        }

        public static int GetIDByName(string name)
        {
            return DataBaseHelper.GetObjects<User>()?.FirstOrDefault(x => x.Username == name)?.id ?? 0;
        }

        public static bool PasswordTrue(int id, string pw)
        {
            return pw == DataBaseHelper.GetObjects<User>()?.FirstOrDefault(x => x.id == id)?.Password;
        }
    }
}
