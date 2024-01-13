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

            User user = new User { UsernameTEST = "MyMotherIsYourMother" };
            user.PutUserInDataBase();
            var conversation = DataBaseHelper.GetObjects<Conversation>().First();
            conversation.AddUserToConversation(user);
        }
    }
}