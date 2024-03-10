using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciChatApi.Models
{
	internal class Message
	{
		public int id { get; set; }
		public string Content { get; set; }
		public int UserID { get; set; }
		public int ConversationID { get; set; }
	}
}
