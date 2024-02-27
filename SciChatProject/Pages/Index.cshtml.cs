using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SciChatProject.Models;
using Microsoft.AspNetCore.Http;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Authentication;

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
            HttpContext.Session.Remove("idlogin");
            string url = "/login";
            return Redirect(url);
        }
    }
}