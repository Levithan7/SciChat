using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SciChatProject.Pages
{
	public class loginModel : PageModel
    {
        public IActionResult OnGet()
        {
			if (HttpContext.Session.GetInt32("idlogin") > 0)
            {
                string url = "/Index";
                return Redirect(url);
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            /*var foo = Request.Form["proceedingType"];
            if(!bool.TryParse(Request.Form["proceedingType"], out bool isLogin))
            {
                HttpContext.Session.SetString("error", "Proceeing-Type not set correclty.");
                return Page();
            }*/
            var isLogin = true;

            int userID = Models.User.GetIDByName(Request.Form["name"]);
            string password = Request.Form["password"];

			if (isLogin && userID != 0)
            {
                if(userID == 0)
                {
                    HttpContext.Session.SetString("error", "This user does not exist!");
                    return Page();
                }

                bool passwordToF = Models.User.PasswordTrue(userID, password);
                if (passwordToF == true)
                {
                    HttpContext.Session.SetInt32("idlogin", userID);
					string url = "/Index";
                    return Redirect(url);
                }
                else
                {
                    HttpContext.Session.SetString("error", "The password is wrong.");
                    return Page();
                }
            }

            else if (!isLogin)
            {
                if (userID != 0)
                {
                    HttpContext.Session.SetString("error", "This user does not exist!");
                    return Page();
                }

                Models.User.PutUserInDataBase(password, Request.Form["signupname"]);
                HttpContext.Session.SetInt32("idlogin", Models.User.GetLastUser().id);
                string url = "/Index";
                return Redirect(url);
            }
			return Page();

        }
    }
}
