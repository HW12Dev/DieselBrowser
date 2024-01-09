using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib.DieselTypes
{
    public class PersistentObject
    {
        public string Name;

        /// <summary>
        /// Read a PersistentObject (container for string/idstring)
        /// </summary>
        /// <param name="br"></param>
        /// <param name="Ballistics">Set to true if parsing assets from the best selling video game Ballistics</param>
        public void ReadPersistentObject(BinaryReader br, bool Ballistics)
        {
            // Uses idstring in modern diesel
            if (Ballistics)
            {
                Name = Utils.ReadNullTerminatedString(br);
            } else
            {
                Name = new IdString(br.ReadUInt64()).Source;
            }
        }
    }
}
