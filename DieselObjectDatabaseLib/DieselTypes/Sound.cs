using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
	public class Sound
	{
		public uint Unknown;
		public uint SoundParserRefId;

		public static Sound Read(BinaryReader br)
		{
			Sound sound = new Sound();

			// could be a mode or format of some sort
			sound.Unknown = br.ReadUInt32();

			sound.SoundParserRefId = br.ReadUInt32();

			return sound;
		}
	}
}
