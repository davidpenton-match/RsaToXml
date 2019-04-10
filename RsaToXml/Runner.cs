using System;
using System.IO;
using RsaToXml.Converter;

namespace RsaToXml
{
	public static class Runner
	{
		public static void Run(string[] files)
		{
			if (files == null)
				throw new ArgumentNullException("files");

			var l = files.Length;

			for(int i = 0; i < l; i++)
				Run(files[i]);
		}

		private static void Run(string file)
		{
			Console.WriteLine();
			Console.WriteLine("Processing file:");
			Console.WriteLine(file);

			var newFileName = Path.GetFileNameWithoutExtension(file) + ".xml";

			if (File.Exists(newFileName))
			{
				Console.WriteLine("Do you want to overwrite this file?");
				Console.WriteLine(newFileName);
				Console.WriteLine("Enter (y/n)");
				var response = Console.ReadLine();
				if (response?.Trim().StartsWith("y", StringComparison.InvariantCultureIgnoreCase) ?? false)
					File.Delete(newFileName);
				else
				{
					Console.WriteLine("File was skipped.");
					return;
				}
			}

			var data = File.ReadAllText(file);

			var converted = data.PemStringToXml();

			var newPath = Path.Combine(Environment.CurrentDirectory, newFileName);
			File.WriteAllText(newPath, converted);

			Console.WriteLine("File converted and saved to path:");
			Console.WriteLine(newPath);
		}
	}
}
