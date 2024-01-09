using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
    public class ParamBlock
    {
        public List<uint> Controllers = new List<uint>();
        public static ParamBlock Read(BinaryReader br)
        {
            ParamBlock paramBlock = new ParamBlock();

            var i = br.ReadUInt32();

            for(; i != 0; --i)
            {
                var controller_refid = br.ReadUInt32();

                paramBlock.Controllers.Add(controller_refid);
            }

            br.BaseStream.Position += 8;

            return paramBlock;
        }
    }
    public class Object3D : PersistentObject
    {
        public ParamBlock ParamBlock;

        // todo: figure out what pd is
        // todo: write a datatype for matrix and vector3
        byte[] pd;
        byte[] Local_TM;
        byte[] Vec3;

        public uint Parent;
        public Object3D ReadObject3D(BinaryReader br, bool Ballistics)
        {
            this.ReadPersistentObject(br, Ballistics);

            if (!Ballistics)
            {
                //this.ParamBlock = ParamBlock.Read(br);
            } else if(Ballistics)
            {
                // matrix 1
                this.pd = br.ReadBytes(0x40);

                // matrix 2
                this.Local_TM = br.ReadBytes(0x40);

                this.Vec3 = br.ReadBytes(0xC);

                this.Parent = br.ReadUInt32();
            }

            // return current object so var variable = new Object3D().Read(...) can be done
            return this;
        }
    }
}
