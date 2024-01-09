using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib
{
	public class IdString
	{
		private static Dictionary<ulong, string> hashes = new Dictionary<ulong, string>();

		public string Source = "Not in hashlist";
		public ulong Hash = 0UL;

		public IdString(string input)
		{
			Source = input;
			Hash = Hash64.HashString(input);
			hashes[Hash] = Source;
		}

		public IdString(ulong hash)
		{
			Hash = hash;

			if (hashes.ContainsKey(Hash)) {
				Source = hashes[Hash];
			} else
			{
				Source = "Not in hashlist (" + hash.ToString("X").ToLower() + ")";
			}
		}

		public static void LoadHashlist(string[] hashlist) {
			foreach (string hash in hashlist)
			{
				if(hash != "")
					hashes[Hash64.HashString(hash)] = hash;
			}
		}
	}
}
