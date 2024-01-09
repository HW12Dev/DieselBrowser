using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
	public class GeneralSoundParser
	{
		public string Source;
		public static GeneralSoundParser Read(BinaryReader br, bool Ballistics = true)
		{
			GeneralSoundParser parser = new GeneralSoundParser();
			if(Ballistics) {
				parser.Source = Utils.ReadNullTerminatedString(br);
			}
			return parser;
		}
	}
}
