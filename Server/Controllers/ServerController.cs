using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SciChatProject;
using SciChatProject.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        [HttpGet("convByID")]
        public IActionResult GetConversation(int id)
        {
            if(!SciChatProject.Models.User.PasswordTrue(u, p))
                return Unauthorized($"Wrong credentials for user {u}");

            var conversation = Conversation.GetConversationByID(id);
            if (!conversation.GetUsers().Select(x => x.id).Contains(id))
                return Unauthorized($"The bot is not part of the Conversation {id} and thus not allowed to access it");
            
            var json = JsonSerializer.Serialize(conversation);
            return Ok(json);
        }

        [HttpGet("convsByUserID")]
        public IActionResult GetConversationsByUserID(int userid)
        {
			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

            if(userid != u)
                return Unauthorized($"The bot {u} is not allowed to access the conversations of user {userid}");

			var json = JsonSerializer.Serialize(SciChatProject.Models.User.GetUserByID(userid).GetConversations());
            return Ok(json);
        }

        [HttpGet("convMembersByID")]
        public IActionResult GetConversationMembers(int id) 
        {
			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			var conversation = Conversation.GetConversationByID(id);
			if (!conversation.GetUsers().Select(x => x.id).Contains(id))
				return Unauthorized($"The bot is not part of the Conversation {id} and thus not allowed to access it");

			var json = JsonSerializer.Serialize(Conversation.GetConversationByID(id).GetUsers());
            return Ok(json);
        }

        [HttpGet("convMessagesByID")]
        public IActionResult GetConversationMessages(int id)
        {
			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			var conversation = Conversation.GetConversationByID(id);
			if (!conversation.GetUsers().Select(x => x.id).Contains(id))
				return Unauthorized($"The bot is not part of the Conversation {id} and thus not allowed to access it");

			var json = JsonSerializer.Serialize(Conversation.GetConversationByID(id).GetMessages());
            return Ok(json);
        }

        [HttpGet("userReceivedMessagesByID")]
        public IActionResult GetUserReceivedMessages(int userid, bool includeOwn=false)
        {
            var messages = SciChatProject.Models.User.GetUserByID(userid).GetConversations().SelectMany(x=>x.GetMessages()).ToList();
            if (!includeOwn)
                messages.RemoveAll(x=>x.UserID==userid);
            var json = JsonSerializer.Serialize(messages);
            return Ok(json);
        }

        [HttpGet("userSentMessagesByID")]
        public IActionResult GetUserSentMessages(int userid)
        {
			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");
			if (userid != u)
				return Unauthorized($"The bot {u} is not allowed to access the conversations of user {userid}");

			var messages = SciChatProject.Models.User.GetUserByID(userid).GetMessages();
            var json = JsonSerializer.Serialize(messages);
            return Ok(json);
        }

        [HttpPost("addmessage")]
        public IActionResult AddMessage(int userid, int conversationid, string content)
        {
			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			if (userid != u)
				return Unauthorized($"The bot {u} is not allowed to access the conversations of user {userid}");

			var conversation = Conversation.GetConversationByID(conversationid);
			if (!conversation.GetUsers().Select(x => x.id).Contains(conversationid))
				return Unauthorized($"The bot is not part of the Conversation {conversationid} and thus not allowed to access it");

			Message.SendMessage(content, userid, conversationid);
            return Ok("Message Sent!");
        }
    }
}
