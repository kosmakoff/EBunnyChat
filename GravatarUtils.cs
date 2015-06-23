using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EBunnyChat
{
	public static class GravatarUtils
	{
		private static readonly MD5 MD5Hash = MD5.Create();

		public static string GetGravatarUri(string email, int? size = null)
		{
			byte[] hashBytes = MD5Hash.ComputeHash(Encoding.Default.GetBytes(email));
			
			var uri = new StringBuilder();

			uri.Append("http://www.gravatar.com/avatar/");

			for (int i = 0; i < hashBytes.Length; i++)
			{
				uri.Append(hashBytes[i].ToString("x2"));
			}

			if (size.HasValue)
			{
				uri.Append("?s=");
				uri.Append(size.Value);
			}

			return uri.ToString();
		}
	}
}
