using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
	public class LightSet : PersistentObject
	{
		List<uint> Lights = new List<uint>();
		public static LightSet Read(BinaryReader br, bool Ballistics) {
			LightSet result = new LightSet();

			result.ReadPersistentObject(br, Ballistics);

			var s = br.ReadUInt32();

			for(int i = 0; i < s; i++)
			{
				var id = br.ReadUInt32();

				result.Lights.Add(id);
			}

			return result;
		}
	}
}
