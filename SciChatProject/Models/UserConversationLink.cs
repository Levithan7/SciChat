namespace SciChatProject.Models
{
    public class UserConversationLink : SQLClass
    {
        public static string? TableName { get; set; } = "user_conversation_link";

        [SQLProperty(Name="userid")] public int UserID { get; set; }
        [SQLProperty(Name="conversationid")] public int ConversationID { get; set; }
    }
}
