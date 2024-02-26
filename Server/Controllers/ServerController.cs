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
            var json = JsonSerializer.Serialize(Conversation.GetConversationByID(id));
            return Ok(json);
        }

        [HttpGet("convsByUserID")]
        public IActionResult GetConversationsByUserID(int userid)
        {
            var json = JsonSerializer.Serialize(SciChatProject.Models.User.GetUserById(userid).GetConversations());
            return Ok(json);
        }

        [HttpGet("convMembersByID")]
        public IActionResult GetConversationMembers(int id) 
        {
            var json = JsonSerializer.Serialize(Conversation.GetConversationByID(id).GetUsers());
            return Ok(json);
        }

        [HttpGet("convMessagesByID")]
        public IActionResult GetConversationMessages(int id)
        {
            var json = JsonSerializer.Serialize(Conversation.GetConversationByID(id).GetMessages());
            return Ok(json);
        }

        [HttpGet("userReceivedMessagesByID")]
        public IActionResult GetUserReceivedMessages(int userid, bool includeOwn=false)
        {
            var messages = SciChatProject.Models.User.GetUserById(userid).GetConversations().SelectMany(x=>x.GetMessages()).ToList();
            if (!includeOwn)
                messages.RemoveAll(x=>x.UserID==userid);
            var json = JsonSerializer.Serialize(messages);
            return Ok(json);
        }

        [HttpGet("userSentMessagesByID")]
        public IActionResult GetUserSentMessages(int userid)
        {
            var messages = SciChatProject.Models.User.GetUserById(userid).GetMessages();
            var json = JsonSerializer.Serialize(messages);
            return Ok(json);
        }

        [HttpPost("addmessage")]
        public IActionResult AddMessage(int userid, int conversationid, string content)
        {
            Message.SendMessage(content, userid, conversationid);
            return Ok("Message Sent!");
        }
    }
}
