using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DieselObjectDatabaseLib.DieselTypes.D3DShaderPass;

namespace DieselObjectDatabaseLib.DieselTypes
{
    public enum wd3dStateVariableType
    {
        CONSTANT = 0x0,
        REFERENCE = 0x1,
        UNINITIALIZED = 0x2,
    }
    public class wd3dStateVariable
    {
        wd3dStateVariable() { }

        public wd3dStateVariableType VarType = wd3dStateVariableType.UNINITIALIZED;
        public ulong Reference;
        public uint Constant;

        public static wd3dStateVariable Read(BinaryReader br)
        {
            wd3dStateVariable result = new wd3dStateVariable();

            result.VarType = (wd3dStateVariableType)br.ReadByte();

            if (result.VarType == wd3dStateVariableType.REFERENCE)
            {
                result.Reference = br.ReadUInt64(); // IDSTRING
            }
            else
            {
                // type is CONSTANT
                result.Constant = br.ReadUInt32();
            }

            return result;
        }
        public static wd3dStateVariable Make(wd3dStateVariableType varType, ulong reference, uint constant)
        {
            wd3dStateVariable ret = new wd3dStateVariable();

            ret.VarType = varType;

            if (varType == wd3dStateVariableType.REFERENCE)
            {
                ret.Reference = reference;
            }
            else
            {
                ret.Constant = constant;
            }

            return ret;
        }
        public void Write(BinaryWriter bw)
        {
            bw.Write((byte)VarType);

            if (VarType == wd3dStateVariableType.REFERENCE)
            {
                bw.Write(Reference);
            }
            else
            {
                bw.Write(Constant);
            }
        }
    }
    public struct wd3dD3DStateBlock
    {
        public wd3dD3DStateBlock() { }
        public List<Tuple<uint, wd3dStateVariable>> states = new List<Tuple<uint, wd3dStateVariable>>();

        public int modern;
    }
    public class D3DShaderPass
    {
        public wd3dD3DStateBlock render_states = new wd3dD3DStateBlock();
        public List<Tuple<uint, wd3dD3DStateBlock>> sampler_state_blocks = new List<Tuple<uint, wd3dD3DStateBlock>>();
        public List<byte> compiled_vertex_shader = new List<byte>();
        public List<byte> compiled_pixel_shader = new List<byte>();

        public void Write(BinaryWriter bw, bool Modern, bool HeaderOnly)
        {

            bw.Write(render_states.states.Count);

            foreach (var keystatepair in render_states.states)
            {
                // pair_key
                bw.Write(keystatepair.Item1);

                keystatepair.Item2.Write(bw);
            }

            bw.Write(sampler_state_blocks.Count);

            foreach (var samplerstatepair in sampler_state_blocks)
            {
                // pair_key
                bw.Write(samplerstatepair.Item1);

                if (Modern)
                {
                    bw.Write(samplerstatepair.Item2.modern);
                }
                bw.Write(samplerstatepair.Item2.states.Count);

                foreach (var statekeypair in samplerstatepair.Item2.states)
                {
                    // state_pair_key
                    bw.Write(statekeypair.Item1);

                    statekeypair.Item2.Write(bw);
                }
            }

            if (!HeaderOnly)
            {
                bw.Write(compiled_vertex_shader.Count);
                bw.Write(compiled_vertex_shader.ToArray());

                bw.Write(compiled_pixel_shader.Count);
                bw.Write(compiled_pixel_shader.ToArray());
            }
        }

        public static D3DShaderPass Read(BinaryReader br, bool Modern, bool HeaderOnly)
        {
            D3DShaderPass shaderPass = new D3DShaderPass();

            var s = br.ReadUInt32();

            // states
            for (int i = 0; i < s; i++)
            {
                uint pair_key = br.ReadUInt32();

                var stateVariable = wd3dStateVariable.Read(br);

                shaderPass.render_states.states.Add(Tuple.Create(pair_key, stateVariable));
            }

            s = br.ReadUInt32();

            // sampler state blocks
            for (int i = 0; i < s; i++)
            {
                var block = new wd3dD3DStateBlock();

                var pair_key = br.ReadUInt32();
                if (Modern)
                {
                    // Modern versions of the engine have an extra int32 here
                    block.modern = br.ReadInt32();
                }
                var num_states = br.ReadInt32();

                for (int j = 0; j < num_states; j++)
                {
                    var state_pair_key = br.ReadUInt32();

                    var stateVariable = wd3dStateVariable.Read(br);

                    block.states.Add(Tuple.Create(state_pair_key, stateVariable));
                }

                shaderPass.sampler_state_blocks.Add(Tuple.Create(pair_key, block));
            }

            // actual shaders now

            if (!HeaderOnly)
            {
                // vertex shader
                var vertex_length = br.ReadUInt32();
                var vertex_start_offset = br.BaseStream.Position;
                while (br.BaseStream.Position < vertex_start_offset + vertex_length)
                {
                    shaderPass.compiled_vertex_shader.Add(br.ReadByte());
                }

                // pixel shader
                var pixel_length = br.ReadUInt32();
                var pixel_start_offset = br.BaseStream.Position;
                while (br.BaseStream.Position < pixel_start_offset + pixel_length)
                {
                    shaderPass.compiled_pixel_shader.Add(br.ReadByte());
                }
            }

            br.Close();

            return shaderPass;
        }
    }
}
