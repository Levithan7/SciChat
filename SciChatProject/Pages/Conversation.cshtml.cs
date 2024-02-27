using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
<<<<<<< Updated upstream
=======
using SciChatProject.Models;
using ScottPlot.Panels;
using Microsoft.AspNetCore.Http;
>>>>>>> Stashed changes

namespace SciChatProject.Pages
{
    public class ConversationModel : PageModel
    {
<<<<<<< Updated upstream
        public void OnGet()
        {
        
        }
    }
=======
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
>>>>>>> Stashed changes
}
