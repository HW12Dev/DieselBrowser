using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
	public class SoundObject3D : Object3D
	{
		public byte[] Unknown;

		// RefId to a dsl::Sound object
		public uint SoundRefId;
		public static SoundObject3D Read(BinaryReader br, bool Ballistics) {
			SoundObject3D soundObject = new SoundObject3D();

			soundObject.ReadObject3D(br, Ballistics);

			soundObject.Unknown = br.ReadBytes(0x18);
			soundObject.SoundRefId = br.ReadUInt32();

			return soundObject;
		}
	}
}
