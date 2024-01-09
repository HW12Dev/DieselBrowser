using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
	public class SimpleTexture
	{
		public uint ImageParser;
		public static SimpleTexture Read(BinaryReader br)
		{
			SimpleTexture texture = new SimpleTexture();

			br.BaseStream.Position += 8;
			texture.ImageParser = br.ReadUInt32();

			return texture;
		}
	}
}
