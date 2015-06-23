using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EBunnyChat
{
	[Serializable]
	public class ChatMessage
	{
		public string Email { get; set; }
		public string Username { get; set; }
		public string Text { get; set; }
	}
}
