using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
    public class Material : PersistentObject
    {
        /// <summary>
        /// List of refids for each texture
        /// </summary>
        public List<uint> Textures = new List<uint>();

        List<byte> SkippedBytes = new List<byte>();

        public static Material Read(BinaryReader br, bool Ballistics)
        {
            Material material = new Material();

            material.ReadPersistentObject(br, Ballistics);

            // Skip 48 bytes, just because. But save it, just in case
            //var end_of_skipped_bytes = br.BaseStream.Position + 48;
            //while (br.BaseStream.Position < end_of_skipped_bytes)
            //{
            //	material.SkippedBytes.Add(br.ReadByte());
            //}

            /*var n = br.ReadInt32();

			for(int i = 0; i < n; i++)
			{
				// goes unused in LAG
				var pd = br.ReadInt32();

				var texture_refid = br.ReadUInt32();

				material.Textures.Add(texture_refid);
			}*/

            return material;
        }
    }
}
