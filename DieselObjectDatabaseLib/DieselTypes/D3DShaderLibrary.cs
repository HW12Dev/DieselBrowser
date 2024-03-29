﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
    public class D3DShaderLibrary
    {
        public Dictionary<uint, ulong> idstring_refid = new Dictionary<uint, ulong>();
        public static D3DShaderLibrary Read(BinaryReader br, bool Modern)
        {
            D3DShaderLibrary shaderLibrary = new();

            var s = br.ReadInt32();

            for (int i = 0; i < s; i++)
            {
                var idstring = 0ul;
                if (Modern)
                {
                    idstring = br.ReadUInt64();
                } else if (Modern == false)
                {
                    idstring = new IdString(Utils.ReadNullTerminatedString(br)).Hash;
                }
                var refid = br.ReadUInt32();

                shaderLibrary.idstring_refid[refid] = idstring;
            }

            return shaderLibrary;
        }
    }
}
