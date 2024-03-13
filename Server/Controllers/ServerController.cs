using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SciChatProject;
using SciChatProject.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualBasic;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        // u (user ID) and p (user password) are used for authentication

        [HttpGet("convByID")]
        public IActionResult GetConversation(int id, int u, string p)
        {
            if(!SciChatProject.Models.User.PasswordTrue(u, p))
                return Unauthorized($"Wrong credentials for user {u}");

			var conversation = Conversation.GetConversationByID(id);
			if (!conversation.GetUsers().Select(x => x.id).Contains(u))
				return Unauthorized($"The bot is not part of the Conversation {id} and thus not allowed to access it");

			var json = JsonSerializer.Serialize(Conversation.GetConversationByID(id));
            return Ok(json);
        }

        [HttpGet("convsByUserID")]
        public IActionResult GetConversationsByUserID(int userid, int u, string p)
        {
            if (userid != u)
                return Unauthorized($"The bot (ID: {u}) trying to access the conversation of the user (ID: {userid}) does not match its ID");

			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			var json = JsonSerializer.Serialize(SciChatProject.Models.User.GetUserByID(userid).GetConversations());
            return Ok(json);
        }

        [HttpGet("convMembersByID")]
        public IActionResult GetConversationMembers(int id, int u, string p) 
        {
			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			var conversation = Conversation.GetConversationByID(id);
			if (!conversation.GetUsers().Select(x => x.id).Contains(u))
				return Unauthorized($"The bot is not part of the Conversation {id} and thus not allowed to access it");

			var json = JsonSerializer.Serialize(Conversation.GetConversationByID(id).GetUsers());
            return Ok(json);
        }

        [HttpGet("convMessagesByID")]
        public IActionResult GetConversationMessages(int id, int u, string p)
        {
			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			var conversation = Conversation.GetConversationByID(id);
			if (!conversation.GetUsers().Select(x => x.id).Contains(u))
				return Unauthorized($"The bot is not part of the Conversation {id} and thus not allowed to access it");

			var json = JsonSerializer.Serialize(Conversation.GetConversationByID(id).GetMessages());
            return Ok(json);
        }

        [HttpGet("userReceivedMessagesByID")]
        public IActionResult GetUserReceivedMessages(int userid, int u, string p, bool includeOwn=false)
        {
			if (userid != u)
				return Unauthorized($"The bot (ID: {u}) trying to access the conversation of the user (ID: {userid}) does not match its ID");

			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			var messages = SciChatProject.Models.User.GetUserByID(userid).GetConversations().SelectMany(x=>x.GetMessages()).ToList();
            if (!includeOwn)
                messages.RemoveAll(x=>x.UserID==userid);

            var json = JsonSerializer.Serialize(messages);
            return Ok(json);
        }

        [HttpGet("userSentMessagesByID")]
        public IActionResult GetUserSentMessages(int userid, int u, string p)
        {
			if (userid != u)
				return Unauthorized($"The bot (ID: {u}) trying to access the conversation of the user (ID: {userid}) does not match its ID");

			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			var messages = SciChatProject.Models.User.GetUserByID(userid).GetMessages();
            var json = JsonSerializer.Serialize(messages);
            return Ok(json);
        }

        [HttpPost("addmessage")]
        public IActionResult AddMessage(int userid, int conversationid, string content, int u, string p)
        {
			if (userid != u)
				return Unauthorized($"The bot (ID: {u}) trying to access the conversation of the user (ID: {userid}) does not match its ID");

			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			var conversation = Conversation.GetConversationByID(conversationid);
			if (!conversation.GetUsers().Select(x => x.id).Contains(u))
				return Unauthorized($"The bot is not part of the Conversation {conversationid} and thus not allowed to access it");

			Message.SendMessage(content, userid, conversationid);
            return Ok("Message Sent!");
        }
    }
}
