using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
	public class Light : Object3D
	{
		public enum Type
		{
			type_omni = 0x1,
			type_spot = 0x2,
			type_directional = 0x3,
			type_clipped_omni = 0x4,
			type_ambient_cube = 0x5
		}

		public bool Enable;
		public Type LightType;
		public static Light Read(BinaryReader br, bool Ballistics)
		{
			Light light = new Light();

			light.Enable = br.ReadBoolean();

			// TODO: complete dsl::Light::load

			return light;
		}
	}
}
