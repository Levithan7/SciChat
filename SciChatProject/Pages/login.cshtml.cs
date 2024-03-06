using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SciChatProject.Pages
{
    public class loginModel : PageModel
    {
        public void OnGet()
        {
            
        }

        public IActionResult OnPost()
        {
            string u = string.Empty;
            string pass = string.Empty;
            int user = 0;
            try
            {
                u = Request.Form["signupname"];
                pass = Request.Form["signuppassword"];
                user = Models.User.GetIDByName(Request.Form["loginname"]);
            }
            catch
            {
            }
            if(user != 0 )
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
            
            if (string.Empty.Equals(u) == false)
            {
                Models.User.PutUserInDataBase(pass, u);
                HttpContext.Session.SetInt32("idlogin", Models.User.GetLastUser().id);
                string url = "/Index";
                return Redirect(url);
            }
            return Page();

        }
    }
}
