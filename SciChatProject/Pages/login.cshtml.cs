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
            int user = Models.User.GetIDByID(Int32.Parse(Request.Form["loginid"]));
            if(user != 0 )
            {
                bool passwordToF = Models.User.PasswordTrue(Int32.Parse(Request.Form["loginid"]), Request.Form["loginpassword"]);
                if (passwordToF == true)
                {
                    HttpContext.Session.SetInt32("idlogin", Int32.Parse(Request.Form["loginid"]));
                    string url = "/Index";
                    return Redirect(url);

                }
                else
                {
                    HttpContext.Session.SetString("error", "The password is wrong.");
                    return Page();
                }
            }
            else
            {
                HttpContext.Session.SetString("error", "The id isnt set yet.");
                return Page();
            }
            
        }
    }
}
