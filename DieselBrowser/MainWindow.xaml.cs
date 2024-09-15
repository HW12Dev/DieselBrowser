using DieselObjectDatabaseLib;
using DieselObjectDatabaseLib.DieselTypes;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Ookii.Dialogs.Wpf;

namespace DieselBrowser
{
    public class DisplayType
	{
		public uint RefId { get; set; }
		public string ItemType { get; set; }
		public uint Size { get; set; }

		public string PossibleName { get; set; }

		public string ShortSourceFile { get; set; }

		public string SourceFile { get; set; }
		public List<byte> Data { get; set; }
		public uint TypeId { get; set; }
	}
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		VistaFolderBrowserDialog OpenFolderDialog = new VistaFolderBrowserDialog();
		VistaOpenFileDialog OpenFileDialog = new VistaOpenFileDialog();

		VistaFolderBrowserDialog ExportFolderDialog = new VistaFolderBrowserDialog();
		VistaSaveFileDialog ExportFileDialog = new VistaSaveFileDialog();

		VistaFolderBrowserDialog ImportFolderDialog = new VistaFolderBrowserDialog();
		VistaOpenFileDialog ImportFileDialog = new VistaOpenFileDialog();

		VistaSaveFileDialog SaveDatabaseFileDialog = new VistaSaveFileDialog();

		List<string> OpenDatabases = new List<string>();
		public MainWindow()
		{
			InitializeComponent();

			if(File.Exists("./hashlist"))
			{
				IdString.LoadHashlist(File.ReadAllLines("./hashlist"));
			}
		}

		private List<DisplayType> PopulateNamesForEntries(List<DisplayType> displayList, bool Modern)
		{
			List<DisplayType> newlist = displayList;

			foreach(var entry in displayList) {
				if(entry.ItemType == "D3DShaderLibrary")
				{
					BinaryReader reader = new BinaryReader(new MemoryStream(entry.Data.ToArray()));
					var shaderLibrary = D3DShaderLibrary.Read(reader, Modern);

					foreach(var pair in shaderLibrary.idstring_refid)
					{
						newlist.Find(e => e.RefId == pair.Key && e.ShortSourceFile == entry.ShortSourceFile).PossibleName = new IdString(pair.Value).Source;
					}
				}
				if(entry.ItemType == "D3DShader")
				{
					BinaryReader reader = new BinaryReader(new MemoryStream(entry.Data.ToArray()));
					var shader = D3DShader.Read(reader, Modern);

					foreach(var layer in shader.Layers) {
						var hash_name = new IdString(layer.Key).Source;

						foreach(var refid in layer.Value)
						{
							newlist.Find(e => e.RefId == refid && e.ShortSourceFile == entry.ShortSourceFile).PossibleName = hash_name;
						}
					}
				}

				if(entry.ItemType == "Material")
				{
					BinaryReader reader = new BinaryReader(new MemoryStream(entry.Data.ToArray()));

					// ballistics is one of the only games that uses .diesel, all the others use .shaders
					bool ballistics = entry.ShortSourceFile.ToLower().Contains(".diesel");

					var material = Material.Read(reader, ballistics);

					newlist.Find(e => e.RefId == entry.RefId && e.ShortSourceFile == entry.ShortSourceFile).PossibleName = material.Name;
				}

				if(entry.ItemType == "Object3D")
				{
					BinaryReader reader = new BinaryReader(new MemoryStream(entry.Data.ToArray()));

					bool ballistics = entry.ShortSourceFile.ToLower().Contains(".diesel");

					var object3d = new Object3D().ReadObject3D(reader, ballistics);

					newlist.Find(e => e.RefId == entry.RefId && e.ShortSourceFile == entry.ShortSourceFile).PossibleName = object3d.Name;
				}

				if(entry.ItemType == "GeneralImageParser")
				{
					BinaryReader reader = new BinaryReader(new MemoryStream(entry.Data.ToArray()));

					bool ballistics = entry.ShortSourceFile.ToLower().Contains(".diesel");

					var imageParser = GeneralImageParser.Read(reader, ballistics);

					newlist.Find(e => e.RefId == entry.RefId && e.ShortSourceFile == entry.ShortSourceFile).PossibleName = imageParser.Name;
				}

				if(entry.ItemType == "SoundObject3D")
				{
					BinaryReader reader = new BinaryReader(new MemoryStream(entry.Data.ToArray()));

					bool ballistics = entry.ShortSourceFile.ToLower().Contains(".diesel");

					var soundObject3d = SoundObject3D.Read(reader, ballistics);

					newlist.Find(e => e.RefId == entry.RefId && e.ShortSourceFile == entry.ShortSourceFile).PossibleName = soundObject3d.Name;
				}
				if(entry.ItemType == "GeneralSoundParser")
				{
					BinaryReader reader = new BinaryReader(new MemoryStream(entry.Data.ToArray()));

					bool ballistics = entry.ShortSourceFile.ToLower().Contains(".diesel");

					var soundParser = GeneralSoundParser.Read(reader, ballistics);
					newlist.Find(e => e.RefId == entry.RefId && e.ShortSourceFile == entry.ShortSourceFile).PossibleName = soundParser.Source;
				}
			}

			return newlist;
		}

		private void OpenFolder(string folder)
		{
			OpenDatabases = new List<string>();

			List<DisplayType> displayList = new List<DisplayType>();
			List<string> possibleObjectDatabases = new List<string>();
			possibleObjectDatabases.AddRange(System.IO.Directory.GetFiles(folder, "*.shaders", SearchOption.AllDirectories));
			possibleObjectDatabases.AddRange(System.IO.Directory.GetFiles(folder, "*.diesel", SearchOption.AllDirectories));
			foreach (string file in possibleObjectDatabases)
			{
				var objDb = ObjectDatabase.Read(file);
				OpenDatabases.Add(file.Replace("\\", "/").Split('/').Last());
				foreach (var obj in objDb.Objects)
				{
					DisplayType displayType = new DisplayType()
					{
						RefId = obj.RefId,
						ItemType = obj.ReadableTypeName,
						Size = obj.DataSize,
						SourceFile = file,
						ShortSourceFile = file.Replace("\\", "/").Split('/').Last(),
						Data = obj.RawData,
						TypeId = obj.TypeId
					};

					displayList.Add(displayType);
				}
				objDb.Close();
			}
			var Modern = MessageBox.Show("Are these ObjectDatabases for a game pre-PAYDAY: The Heist?", "DieselBrowser", MessageBoxButton.YesNo) == MessageBoxResult.No;

			displayList = PopulateNamesForEntries(displayList, Modern);

			fileListView.ItemsSource = displayList;
		}

		private void OpenFile(string file)
		{
			OpenDatabases = new List<string>();
			OpenDatabases.Add(file.Replace("\\", "/").Split('/').Last());
			List<DisplayType> displayList = new List<DisplayType>();
			var objDb = ObjectDatabase.Read(file);
			foreach (var obj in objDb.Objects)
			{
				DisplayType displayType = new DisplayType()
				{
					RefId = obj.RefId,
					ItemType = ObjectDbTypeHandler.GetReadableNameFromTypeId(obj.TypeId),
					Size = obj.DataSize,
					SourceFile = file,
					ShortSourceFile = file.Replace("\\", "/").Split('/').Last(),
					Data = obj.RawData,
					TypeId = obj.TypeId
				};

				displayList.Add(displayType);
			}
			objDb.Close();

			var Modern = MessageBox.Show("Is this ObjectDatabase for a game pre-PAYDAY: The Heist?", "DieselBrowser", MessageBoxButton.YesNo) == MessageBoxResult.No;

			displayList = PopulateNamesForEntries(displayList, Modern);

			fileListView.ItemsSource = displayList;
		}

		private void OpenFolder_Click(object sender, EventArgs e)
		{
			if((bool)OpenFolderDialog.ShowDialog(this))
			{
				OpenFolder(OpenFolderDialog.SelectedPath);
			}
		}

		private void OpenFile_Click(object sender, EventArgs e)
		{
			if ((bool)OpenFileDialog.ShowDialog(this))
			{
				OpenFile(OpenFileDialog.FileName);
			}
		}

		private void New_Click(object sender, EventArgs e)
		{
			OpenDatabases = new List<string>();
			OpenDatabases.Add("new.diesel");
			List<DisplayType> displayList = new List<DisplayType>();
			fileListView.ItemsSource = displayList;
		}

		private void Import_Click(object sender, EventArgs e)
		{
			if (OpenDatabases.Count == 0)
			{
				MessageBox.Show("Please open a database before importing");
				return;
			}
			if (OpenDatabases.Count > 1)
			{
				MessageBox.Show("Saving/importing is only supported with one database open at a time");
				return;
			}

			ImportFileDialog.Multiselect = true;

			if (!(bool)ImportFileDialog.ShowDialog(this))
				return;

			foreach (var to_import2 in ImportFileDialog.FileNames)
			{
				var to_import = to_import2.Replace("\\", "/");
				var filename = to_import.Split("/").Last();

				var refid = UInt32.Parse(filename.Split(".")[0]);

				var typeid = ObjectDbTypeHandler.GetTypeIdFromReadableName(Path.GetExtension(filename).Replace(".", ""));

				var data = File.ReadAllBytes(to_import).ToList();

				if (((List<DisplayType>)fileListView.ItemsSource).Find(o => o.RefId == refid) != null)
				{
					((List<DisplayType>)fileListView.ItemsSource).Find(o => o.RefId == refid).Data = data;
					((List<DisplayType>)fileListView.ItemsSource).Find(o => o.RefId == refid).Size = (uint)data.Count();
					((List<DisplayType>)fileListView.ItemsSource).Find(o => o.RefId == refid).TypeId = typeid;
					((List<DisplayType>)fileListView.ItemsSource).Find(o => o.RefId == refid).ItemType = ObjectDbTypeHandler.GetReadableNameFromTypeId(typeid);
				} else {
					((List<DisplayType>)fileListView.ItemsSource).Add(new()
					{
						TypeId = typeid,
						ItemType = ObjectDbTypeHandler.GetReadableNameFromTypeId(typeid),
						RefId = refid,
						Data = data,
						Size = (uint)data.Count(),
						ShortSourceFile = "new.diesel"
					});
				}
				fileListView.Items.Refresh();
			}
		}

		private void SaveAs_Click(object sender, EventArgs e)
		{
			if(OpenDatabases.Count == 0) {
				MessageBox.Show("Please open a database before saving");
				return;
			}
			if(OpenDatabases.Count > 1)
			{
				MessageBox.Show("Saving/importing is only supported with one database open at a time");
				return;
			}

			SaveDatabaseFileDialog.Filter = "DIESEL ObjectDatabase|*.diesel|Shader ObjectDatabase|*.shaders";
			if(!(bool)SaveDatabaseFileDialog.ShowDialog(0))
			{
				return;
			}

			var output = SaveDatabaseFileDialog.FileName;

			List<DieselObject> dieselObjects = new List<DieselObject>();
			foreach(var item in (List<DisplayType>)fileListView.ItemsSource)
			{
				DieselObject obj = new DieselObject()
				{
					TypeId = item.TypeId,
					RefId = item.RefId,
					DataSize = item.Size,
					RawData = item.Data
				};
				dieselObjects.Add(obj);
			}

			ObjectDatabase.Write(new BinaryWriter(File.Open(output, FileMode.Create)), dieselObjects, true);
		}

		private void SaveFiles(object sender, EventArgs e)
		{
			bool multiple = false;
			string out_path = "";
			if(fileListView.SelectedItems.Count > 1) {
				multiple = true;
				if((bool)ExportFolderDialog.ShowDialog(this))
				{
					out_path = ExportFolderDialog.SelectedPath;
				}
			}
			foreach (var item_o in fileListView.SelectedItems) {
				var item = (DisplayType)item_o;
				if(multiple)
				{
					string out_file = Path.Join(out_path, item.RefId.ToString() + "." + item.ShortSourceFile.ToLower().Replace(".", "_") + "." + item.ItemType.ToLower());
					File.WriteAllBytes(out_file, item.Data.ToArray());
				} else
				{
					var new_outname = ExportFileDialog.FileName.Replace("\\", "/").Split("/");
					new_outname = new_outname.Take(new_outname.Count() - 1).ToArray();
					ExportFileDialog.FileName = Path.Join(string.Join("\\", new_outname), item.RefId.ToString() + "." + item.ShortSourceFile.ToLower().Replace(".", "_") + "." +item.ItemType.ToLower());
					if ((bool)ExportFileDialog.ShowDialog(this))
					{
						out_path = ExportFileDialog.FileName;
						File.WriteAllBytes(out_path, item.Data.ToArray());
					}
				}
			}
		}
	}
}