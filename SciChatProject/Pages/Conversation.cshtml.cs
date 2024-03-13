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
			string addUserName = Request.Form["adduser"];
			string check = Request.Form["leaveCheck"];
			string content = Request.Form["contentmessage"];
			int u=0;
			try{u = Models.User.GetIDByName(addUserName);}catch { }
			
			if (!string.IsNullOrEmpty(addUserName) && u != 0)
				Conversation.GetConversationByID(Int32.Parse(Request.Query["conversationid"])).AddUserToConversation(Models.User.GetUserByID(u));
			
			else if(!string.IsNullOrEmpty(content))
				Message.SendMessage(content, HttpContext.Session.GetInt32("idlogin").Value, Int32.Parse(Request.Query["conversationid"]));
			
            else if (check != null)
            {
				List<UserConversationLink> ucl = new () { new() { UserID = (int)HttpContext.Session.GetInt32("idlogin"), ConversationID = int.Parse(Request.Query["conversationid"]) } };
				DataBaseHelper.ExecuteChange(UserConversationLink.TableName, ucl, DataBaseHelper.ChangeType.Delete);
				string url = "/Index";
                return Redirect(url);
            }
			return Page();
		}
 
	}
}
