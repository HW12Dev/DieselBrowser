using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib
{
	public class Utils
	{
		public static string ReadNullTerminatedString(BinaryReader br)
		{
			string ret = "";

			char ch;
			while ((int)(ch = Convert.ToChar(br.ReadByte())) != 0)
				ret = ret + ch;

			return ret;
		}
	}
}
