using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciChatApi.Models
{
	internal class User
	{
		public int id { get; set; }
		public string? Password { get; set; }
		public string? Username { get; set; }
	}
}
