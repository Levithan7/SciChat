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

            //Message message = new Message { Content=@"I like fractions like this: $$\frac{a}{b}$$ very much!"};
            //message.ParseLaTeX();
        }
    }
}