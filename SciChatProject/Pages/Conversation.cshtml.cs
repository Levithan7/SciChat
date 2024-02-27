using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SciChatProject.Models;
using Microsoft.AspNetCore.Http;

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
		public void OnPost() 
		{
			string content = Request.Form["contentmessage"];
			Models.Message.SendMessage(content, 1, 1);
		}

	}
}
