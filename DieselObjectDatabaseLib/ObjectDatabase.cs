using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DieselObjectDatabaseLib
{
	public class DieselObject
	{
		// Non-diesel parameter
		public long OffsetFromFile { get; set; }
		// Non-diesel parameter
		public long DataOffsetFromFile { get; set; }
		// Non-diesel parameter
		public string ReadableTypeName { get; set; }

		public uint TypeId { get; set; }
		public uint RefId { get; set; }
		public uint DataSize { get; set; }

		public object ActualObject { get; set; }

		public List<byte> RawData { get; set; }
	}
	// Read implementation from dsl::ObjectDatabase::load_content in Lead and Gold: Gangs of the Wild West, which fortunately shipped with a debug symbol
	public class ObjectDatabase
	{
		public int ObjectCount { get; private set; }
		public List<DieselObject> Objects { get; private set; } = new List<DieselObject>();

		// not in ballistics, included for compatibility with LAG branch
		uint Size;

		BinaryReader Reader { get; set; }

		public List<byte> GetRawDataFromRefId(uint RefId)
		{
			return Objects.Find(o => o.RefId == RefId).RawData;
		}

		public static void Write(BinaryWriter bw , List<DieselObject> objects, bool new_)
		{
			if (!new_)
			{
				bw.Write(objects.Count());
			} else
			{
				// Signals newer diesel versions that this is a "new" ObjectDatabase
				bw.Write(0xFFFFFFFF);

				// Size, read by game but never used
				bw.Write(0);

				bw.Write(objects.Count());
			}

			foreach(var obj in objects)
			{
				// Type
				bw.Write(obj.TypeId);
				// RefId
				bw.Write(obj.RefId);
				// Data Size
				bw.Write(obj.DataSize);
				// Data
				bw.Write(obj.RawData.ToArray());
			}

			bw.Close();
		}

		public static ObjectDatabase Read(BinaryReader br) {
			ObjectDatabase objDb = new ObjectDatabase();
			objDb.Reader = br;

			objDb.ObjectCount = br.ReadInt32();

			// Only in modern diesel
			if(objDb.ObjectCount == -1)
			{
				objDb.Size = br.ReadUInt32();
				objDb.ObjectCount = br.ReadInt32();
			}

			// Read objects

			for (int i = 0; i < objDb.ObjectCount; i++) {
				DieselObject obj = new DieselObject();

				obj.OffsetFromFile = br.BaseStream.Position;

				obj.TypeId = br.ReadUInt32();

				obj.ReadableTypeName = ObjectDbTypeHandler.GetReadableNameFromTypeId(obj.TypeId);

				obj.RefId = br.ReadUInt32();
				obj.DataSize = br.ReadUInt32();

				obj.DataOffsetFromFile = br.BaseStream.Position;

				//obj.ActualObject = DieselTypes.ReadTypeFromTypeId(obj.TypeId, br);
				
				if(obj.ActualObject == null) {
					obj.RawData = new List<byte>();
					while(br.BaseStream.Position < obj.DataOffsetFromFile + obj.DataSize)
					{
						obj.RawData.Add(br.ReadByte());
					}
					//br.BaseStream.Position += obj.DataSize;
				}

				objDb.Objects.Add(obj);
			}

			return objDb;
		}

		public void Close()
		{
			Reader.Close();
		}
		public static ObjectDatabase Read(string FileName) {
			return Read(new BinaryReader(File.OpenRead(FileName)));
		}
	}
}
