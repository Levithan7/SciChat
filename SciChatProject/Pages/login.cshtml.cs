using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SciChatProject.Pages
{
    public class loginModel : PageModel
    {
        public IActionResult OnGet()
        {
			string test = "/Conversation?conversationid=1";
            while(HttpContext.Session.GetInt32("idlogin") != 1)
            {
			    HttpContext.Session.SetInt32("idlogin", 1);
            }
            
			return Redirect(test);

			if (HttpContext.Session.GetInt32("idlogin") > 0)
            {
                string url = "/Index";
                return Redirect(url);
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            int u = 0;
            string pass = string.Empty;
            int user = 0;
            u = Models.User.GetIDByName(Request.Form["signupname"]);
            pass = Request.Form["signuppassword"];
            user = Models.User.GetIDByName(Request.Form["loginname"]);
			if (user != 0)
            {
                bool passwordToF = Models.User.PasswordTrue(user, Request.Form["loginpassword"]);
                if (passwordToF == true)
                {
                    HttpContext.Session.SetInt32("idlogin", user);
					string url = "/Index";
                    return Redirect(url);

                }
                else
                {
                    HttpContext.Session.SetString("error", "The password is wrong.");
                    return Page();
                }
            }
            else if (u != 0)
            {
                Models.User.PutUserInDataBase(pass, Request.Form["signupname"]);
                HttpContext.Session.SetInt32("idlogin", Models.User.GetLastUser().id);
                string url = "/Index";
                return Redirect(url);
            }
			return Page();

        }
    }
}
