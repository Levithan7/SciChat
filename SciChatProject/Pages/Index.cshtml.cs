using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SciChatProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Xml.Linq;

namespace SciChatProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private string error = "";

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            DataBaseHelper.ConnectionString = Constants.CONNECTIONSTRING;
            if (HttpContext.Session.GetInt32("idlogin") > 0) {
                return Page();
            }
            string url = "/login";
            return Redirect(url);
        }

        public IActionResult OnPost() 
        {
            string u = string.Empty;
            string c = string.Empty;
            try
            {
                u = Request.Form["adduname"];
                c = Request.Form["addcname"];
            }
            catch
            {
            }
            if(string.Empty.Equals(u) == false && string.Empty.Equals(c) == false)
            {
                int id1 = Models.User.GetIDByName(u);
                int id2 = (int)HttpContext.Session.GetInt32("idlogin");
                Models.Conversation.AddUserToNewConversation(Models.User.GetUserByID(id1),Models.User.GetUserByID(id2), c);

            }
            else
            {
                HttpContext.Session.Remove("idlogin");
                string url = "/login";
                return Redirect(url);
            }
            return Page();           
        }
    }
}