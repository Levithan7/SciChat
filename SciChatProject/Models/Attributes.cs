using System.Runtime.CompilerServices;

namespace SciChatProject.Models
{
    public class SQLProperty : Attribute 
    {
        public string? Name { get; set; }
    }
}
