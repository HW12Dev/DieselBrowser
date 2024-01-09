using DieselObjectDatabaseLib.DieselTypes;
using System;

namespace ShaderPassSplitter
{
    public class Program
	{
		public static void Main(string[] args)
		{
			if(args.Length == 0)
			{
				Console.WriteLine("Usage: \n\tShaderPassSplitter split file.d3dshaderpass [modern]\n\tShaderPassSplitter merge file.d3dshaderpass destination.d3dshaderpass [modern]");
				return;
			}
			var mode = args[0];
			if(mode != "split" && mode != "merge")
			{
				Console.WriteLine("Invalid mode: " + mode);
			}

			if(mode == "split")
			{
				var to_split = args[1];

				var modern = args[2] != null && (args[2] == "modern" || args[2] == "true");

				var pass = D3DShaderPass.Read(new BinaryReader(File.OpenRead(to_split)), modern, false);

				BinaryWriter header = new BinaryWriter(File.OpenWrite(to_split + ".header"));
				pass.Write(header, modern, true);
				header.Close();

				File.WriteAllBytes(to_split + ".pixel.shader", pass.compiled_pixel_shader.ToArray());
				File.WriteAllBytes(to_split + ".vertex.shader", pass.compiled_vertex_shader.ToArray());
			} else if(mode == "merge")
			{
				var input = args[1];
				var output = args[2];
				var modern = args[3] != null && (args[3] == "modern" || args[3] == "true");

				var pass = D3DShaderPass.Read(new BinaryReader(File.OpenRead(input + ".header")), modern, true);
				pass.compiled_pixel_shader = File.ReadAllBytes(input + ".pixel.shader").ToList();
				pass.compiled_vertex_shader = File.ReadAllBytes(input + ".vertex.shader").ToList();

				var writer = new BinaryWriter(File.OpenWrite(output));
				pass.Write(writer, modern, false);
				writer.Close();
			}
		}
	}
}