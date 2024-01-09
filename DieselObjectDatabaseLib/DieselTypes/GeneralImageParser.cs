using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
	public class GeneralImageParser : PersistentObject
	{
		public string Image;

		/// <summary>
		/// GeneralImageParser is not allowed to be serialised by Diesel from atleast LAG onwards
		/// </summary>
		/// <param name="br"></param>
		/// <param name="Ballistics"></param>
		/// <returns></returns>
		public static GeneralImageParser Read(BinaryReader br, bool Ballistics)
		{
			GeneralImageParser parser = new GeneralImageParser();

			parser.ReadPersistentObject(br, Ballistics);

			parser.Image = Utils.ReadNullTerminatedString(br);

			return parser;
		}
	}
}
