using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SciChatProject;
using SciChatProject.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;

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

            var json = JsonSerializer.Serialize(Conversation.GetConversationByID(id));
            return Ok(json);
        }

        [HttpGet("convsByUserID")]
        public IActionResult GetConversationsByUserID(int userid, int u, string p)
        {
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

			var json = JsonSerializer.Serialize(Conversation.GetConversationByID(id).GetUsers());
            return Ok(json);
        }

        [HttpGet("convMessagesByID")]
        public IActionResult GetConversationMessages(int id, int u, string p)
        {
			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			var json = JsonSerializer.Serialize(Conversation.GetConversationByID(id).GetMessages());
            return Ok(json);
        }

        [HttpGet("userReceivedMessagesByID")]
        public IActionResult GetUserReceivedMessages(int userid, int u, string p, bool includeOwn=false)
        {
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
			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			var messages = SciChatProject.Models.User.GetUserByID(userid).GetMessages();
            var json = JsonSerializer.Serialize(messages);
            return Ok(json);
        }

        [HttpPost("addmessage")]
        public IActionResult AddMessage(int userid, int conversationid, string content, int u, string p)
        {
			if (!SciChatProject.Models.User.PasswordTrue(u, p))
				return Unauthorized($"Wrong credentials for user {u}");

			Message.SendMessage(content, userid, conversationid);
            return Ok("Message Sent!");
        }
    }
}
