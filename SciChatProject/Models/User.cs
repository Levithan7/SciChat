namespace SciChatProject.Models
{
    public class User : SQLClass
    {
        public static string? TableName { get; set; } = "users";
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

        public static User GetUserByID(int id)
        {
            return DataBaseHelper.GetObjects<User>().First(x => x.id == id);
        }

        public static User GetLastUser()
        {
            return DataBaseHelper.GetObjects<User>().Last();
        }

        public static int GetIDByID(int id)
        {
            try
            {
                return DataBaseHelper.GetObjects<User>().First(x => x.id == id).id;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public static int GetIDByName(string name)
        {
            try
            {
                return DataBaseHelper.GetObjects<User>().First(x => x.Username == name).id;
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public static bool PasswordTrue(int id, string pw)
        {
            try
            {
                return pw == DataBaseHelper.GetObjects<User>().First(x => x.id == id).Password;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
