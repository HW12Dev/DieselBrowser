using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
    public class D3DShader
    {
        // key is idstring, value is list of refids
        public Dictionary<ulong, List<uint>> Layers = new Dictionary<ulong, List<uint>>();
        public static D3DShader Read(BinaryReader br)
        {
            var shader = new D3DShader();

            var num_layers = br.ReadInt32();

            for (int i = 0; i < num_layers; i++)
            {
                var name = br.ReadUInt64(); // idstring
                shader.Layers[name] = new List<uint>();

                var np = br.ReadInt32();

                for (var j = 0; j < np; j++)
                {
                    var refid = br.ReadUInt32();
                    shader.Layers[name].Add(refid);
                }
            }

            return shader;
        }
    }
}
