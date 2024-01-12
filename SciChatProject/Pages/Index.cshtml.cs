using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SciChatProject.Models;

namespace SciChatProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;


        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            DataBaseHelper.ConnectionString = Constants.CONNECTIONSTRING;
            string testQuery = "SELECT * FROM users";
            var result = DataBaseHelper.GetObjectsByQuery<User>(testQuery);

            testQuery = DataBaseHelper.CreateQueryForChange("users", new List<User> { new User { id = 0, username = "LeviDerEchte" } });
            return;
        }
    }
}