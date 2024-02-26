using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SciChatProject.Models;
using ScottPlot.Panels;

namespace SciChatProject.Pages
{
    public class ConversationModel : PageModel
    {
		public void OnGet()
		{

		}
		public void OnPost() 
		{
			string a = Request.Form["contentmessage"];
			Models.Message.SendMessage(a, 1, 1);
		}

	}
}
