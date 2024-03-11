using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SciChatProject.Models;
using Microsoft.AspNetCore.Http;
using ScottPlot.Hatches;

namespace SciChatProject.Pages
{
    public class ConversationModel : PageModel
    {
		public IActionResult OnGet()
		{
            if (HttpContext.Session.GetInt32("idlogin") > 0)
            {
                return Page();
            }
            string url = "/login";
            return Redirect(url);
        }
		public IActionResult OnPost() 
		{
            string check = Request.Form["leaveCheck"];
            int u = 0;
			try
			{
				u = Models.User.GetIDByName(Request.Form["adduser"]);
            }
			catch
			{
			}
			if (u != 0)
			{
				Models.Conversation.GetConversationByID(Int32.Parse(Request.Query["conversationid"])).AddUserToConversation(Models.User.GetUserByID(u));

			}
            else if (check != null)
            {
                DataBaseHelper.DeleteRowFormDB<UserConversationLink>(UserConversationLink.TableName,(int)HttpContext.Session.GetInt32("idlogin"), Int32.Parse(Request.Query["conversationid"]));
                string url = "/Index";
                return Redirect(url);
            }
			else
			{
				string content = Request.Form["contentmessage"];
				Models.Message.SendMessage(content, HttpContext.Session.GetInt32("idlogin").Value, Int32.Parse(Request.Query["conversationid"]));
			}
			return Page();
		}
 
	}
}
