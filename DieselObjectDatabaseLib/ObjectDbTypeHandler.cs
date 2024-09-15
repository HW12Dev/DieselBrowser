using System.Collections.Generic;

namespace DieselObjectDatabaseLib
{
	public class ObjectDbTypeHandler
	{
		// Taken from Lead and Gold: Gangs of the Wild West, some types could possibly not exist in Ballistics
		public static Dictionary<uint, string> TypeIdToType = new Dictionary<uint, string> {
			{ 0x3C54609C, "Material" },
{ 0x29276B1D, "MaterialGroup" },
{ 0x33552583, "LightSet" },
{ 0x46BF31A7, "Camera" },
{ 0x72B4D37, "SimpleTexture" },
{ 0x7AB072D3, "Geometry" },
{ 0x4C507A13, "Topology" },
{ 0x2C1F096F, "NormalManagingGP" },
{ 0x3B634BD, "TopologyIP" },
{ 0x28DB639A, "BezierVector3Controller" },
{ 0x197345A5, "QuatBezRotationController" },
{ 0x62212D88, "Model" },
{ 0xFFCD100, "Object3D" },
{ 0x648A206C, "QuatLinearRotationController" },
{ 0xFFA13B80, "Light" },
{ 0x2C5D6201, "CubicTexture" },
{ 0x22126DC0, "LookAtRotationController" },
{ 0x5ED2532F, "TextureSpaceGP" },

// Missing from https://github.com/kythyria/payday2-tools-rust/blob/master/doc/object_database.md
{ 0x3532976, "GeneralImageParser" },

// Previously unknown

// Found in Arcade
{ 0x527D4246, "TMController" },
// Found in Arcade
{ 0x29F20029, "VertexAnimationData" },
// Found in Arcade
{ 0x72DF7C64, "SnowParticleSystem" },
// Found in Arcade
{ 0x7F3E6151, "GeneralSoundParser" },
// Found in Arcade
{ 0x728871AC, "Sound" },
// Found in Arcade
{ 0x75DD3382, "SoundObject3D" },
// Found in Arcade
{ 0x145350D0, "Halo" },
// Found in Arcade
{ 0x745349A2, "ShaderParameters" },
// Found in Arcade
{ 0x46BC4278, "BezierGroup" },

//// Shaders
{ 0x12812C1A, "D3DShaderLibrary" },
{ 0x7F3552D1, "D3DShader" },
{ 0x214B1AAF, "D3DShaderPass" }

		};
		public static string GetReadableNameFromTypeId(uint typeId)
		{
			if (TypeIdToType.ContainsKey(typeId))
				return TypeIdToType[typeId];
			return typeId.ToString("X");
		}

		public static uint GetTypeIdFromReadableName(string readableName)
		{
			readableName = readableName.ToLower();

			foreach(var pair in TypeIdToType)
			{
				if (pair.Value.ToLower().Equals(readableName))
				{
					return pair.Key;
				}
			}
			return 0x0;
		}
	}
}